//Defaults added during class generation
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//additional references added for class

//for Twilio API usage
using Twilio;
using Twilio.Http;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;

//add nuget package Twilio by Twilio


namespace BoilerPlateDraft.Code_Repo
{
	public class TwilioMessaging
	{
    //Variables needed for messaging
    //must be assigned - not assigned here in boilerplate
    private static string accountSid;
    private static string authToken;
    private static string sendingPhoneNum;  // format: "+1XXXXXXXXXX" x = digits of number


    //For Sending message to user utilizing Twilio API code
    public static void MessageUser(string message, string phoneNum)
    {
      TwilioClient.Init(accountSid, authToken);

      if (phoneNum != null)
      {
        var msge = MessageResource.Create(
            body: message,
            from: new Twilio.Types.PhoneNumber(sendingPhoneNum),
            to: new Twilio.Types.PhoneNumber(phoneNum));
        Console.WriteLine(msge.Sid);
      }
    }

  }
}