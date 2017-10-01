using System;
using Alexa.NET.Response;

namespace AlexaCore
{
	public class PossibleResponse
	{
		public PossibleResponse(string intentName, string intentAsText, Func<SkillResponse> action)
		{
			IntentName = intentName;
			IntentAsText = intentAsText;
			Action = action;
		}

		public string IntentName { get; set; }

		public string IntentAsText { get; set; }

		public Func<SkillResponse> Action { get; set; }

		public string ParameterValue { get; }

		public static PossibleResponse YesResponse(Func<SkillResponse> action)
		{
			return new PossibleResponse(AlexaContext.IntentNames.YesIntent, "yes", action);
		}

		public static PossibleResponse NoResponse(Func<SkillResponse> action, string parameterValue = "")
		{
			return new PossibleResponse(AlexaContext.IntentNames.NoIntent, "no", action);
		}
	}
}