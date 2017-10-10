﻿using Alexa.NET.Request;
using AlexaCore.Intents;
using Amazon.Lambda.Core;

namespace AlexaCore.Tests.Function
{
    class TestFunction : AlexaFunction
    {
        protected override IntentFactory IntentFactory()
        {
            return new TestFunctionIntentFactory();
        }

        protected override void FunctionInit(AlexaContext alexaContext, IntentParameters parameters)
        {
            AlexaContext.Container.RegisterType("globalItem", () => new TestDataStore("Function"));
        }

        protected override IntentParameters BuildParameters(ILambdaLogger logger, Session session)
        {
            return new TestIntentParameters(logger, session);
        }
    }
}
