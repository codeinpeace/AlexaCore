﻿using System.Collections.Generic;
using System.Linq;
using Alexa.NET.Request;
using Alexa.NET.Response;
using AlexaCore.Intents;

namespace AlexaCore.Tests.Function.Intents
{
    class NoIntent : IntentAsResponse
    {
        public NoIntent(IntentParameters parameters) : base(parameters)
        {
        }

        protected override SkillResponse GetResponseInternal(Dictionary<string, Slot> slots)
        {
            return FindPreviousQuestionResponse(slots);
        }

        protected override CommandDefinition SelectLastCommand()
        {
            return Parameters.CommandQueue.Entries().LastOrDefault(a => a.ExpectsResponse);
        }
    }
}