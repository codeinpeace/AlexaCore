﻿using System;
using System.Collections.Generic;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using AlexaCore.Intents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace AlexaCore
{
    public abstract class AlexaFunction
    {
	    private IntentFactory _intentFactory;
	   
	    protected abstract IntentFactory IntentFactory();

	    protected virtual IntentNames IntentNames()
	    {
		    return null;
		}

        private AlexaContext AlexaContext { get; set; }

        protected virtual bool EnableOperationTimerLogging => true;

		public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
		{
		    IntentParameters parameters;

            using (new OperationTimer(context.Logger.LogLine, "Init", EnableOperationTimerLogging))
		    {
		        _intentFactory = IntentFactory();

		        context.Logger.LogLine("Input: " + JsonConvert.SerializeObject(input));

		        parameters = BuildParameters(context.Logger, input.Session);

		        AlexaContext = new AlexaContext(_intentFactory, IntentNames(), parameters);

		        FunctionInit(AlexaContext, parameters);
		    }

		    SkillResponse innerResponse;

		    using (new OperationTimer(context.Logger.LogLine, "Run function", EnableOperationTimerLogging))
		    {
		        innerResponse = Run(input, parameters);

		        innerResponse.SessionAttributes = parameters.SessionAttributes();

		        parameters.Logger.LogLine("Output: " + JsonConvert.SerializeObject(innerResponse));
		    }

		    using (new OperationTimer(context.Logger.LogLine, "Function complete", EnableOperationTimerLogging))
		    {
		        FunctionComplete(innerResponse);
		    }

		    return innerResponse;
		}

        protected virtual IntentParameters BuildParameters(ILambdaLogger logger, Session session)
        {
            return new IntentParameters(logger, session);
        }

        protected virtual void FunctionInit(AlexaContext alexaContext, IntentParameters parameters)
		{
		}

		protected virtual void FunctionComplete(SkillResponse innerResponse)
		{
		}

		public virtual string NoIntentMatchedText => "No intent matched - intent was {0}";

	    private SkillResponse Run(SkillRequest input, IntentParameters parameters)
	    {
		    AlexaIntent intentToRun = null;

		    Dictionary<string, Slot> slots = null;

		    string intentName = "";

		    if (input.GetRequestType() == typeof(LaunchRequest))
		    {
			    intentToRun = AlexaContext.IntentFactory.LaunchIntent(parameters);

			    slots = new Dictionary<string, Slot>();
		    }
		    else if (input.GetRequestType() == typeof(IntentRequest))
		    {
			    var intentRequest = (IntentRequest)input.Request;

			    var intents = AlexaContext.IntentFactory.Intents(parameters);

			    slots = intentRequest.Intent.Slots;

			    intentName = intentRequest.Intent.Name;

				if (intents.ContainsKey(intentRequest.Intent.Name))
			    {
				    intentToRun = intents[intentRequest.Intent.Name];
			    }
			    else
			    {
				    intentToRun = _intentFactory.HelpIntent(parameters);
			    }
		    }

		    if (intentToRun == null)
		    {
			    return ResponseBuilder.Tell(new PlainTextOutputSpeech { Text = String.Format(NoIntentMatchedText, intentName) });
		    }

		    var skillResponse = intentToRun.GetResponse(slots);

		    skillResponse.Response.ShouldEndSession = intentToRun.ShouldEndSession;

		    parameters.CommandQueue.Enqueue(intentToRun.CommandDefinition());

		    return skillResponse;
	    }
	}
}
