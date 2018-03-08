﻿using System.Collections.Generic;
using Alexa.NET.Request;
using Alexa.NET.Response;

namespace AlexaCore.Intents.Default
{
    public class DefaultCancelIntent : AlexaIntent
	{
		public override string IntentName => AlexaContext.IntentNames.CancelIntent;

		public override bool ShouldEndSession => true;

		protected override SkillResponse GetResponseInternal(Dictionary<string, Slot> slots)
		{
		    return Tell("Goodbye");
		}
	}
}