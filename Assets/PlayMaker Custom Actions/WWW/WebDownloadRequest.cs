// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: download file server
// source https://robots.thoughtbot.com/avoiding-out-of-memory-crashes-on-mobile


using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("WWW")]
	[Tooltip ("Gets data from a url and store it as text or texture or in a file.")]
	public class WebDownloadRequest : FsmStateAction
	{
		[RequiredField]
		[Tooltip ("Url to download data from.")]
		public FsmString url;

		[ActionSection ("Results")]

		[UIHint (UIHint.Variable)]
		[Tooltip ("Gets text from the url.")]
		public FsmString storeText;
		
		[UIHint (UIHint.Variable)]
		[Tooltip ("Gets a Texture from the url.")]
		public FsmTexture storeTexture;
		
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		[Tooltip("Gets Sprite from the url.")]
		public FsmObject storeSprite;
		
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(AudioClip))]
		[Tooltip("Gets Audio from the url.")]
		public FsmObject storeAudio;

		[UIHint (UIHint.Variable)]
		[Tooltip ("Saves the content into a file as is")]
		public FsmString saveInFile;

		[ActionSection ("Feedback")]
		[UIHint (UIHint.Variable)]
		[Tooltip ("Error message if there was an error during the download.")]
		public FsmString errorString;

		[UIHint (UIHint.Variable)] 
		[Tooltip ("How far the download progressed (0-1).")]
		public FsmFloat progress;

		[ActionSection ("Events")] 
		
		[Tooltip ("Event to send when the data has finished loading (progress = 1).")]
		public FsmEvent isDone;
		
		[Tooltip ("Event to send if there was an error.")]
		public FsmEvent isError;


		private UnityWebRequest uwr;

		DownloadHandlerBuffer d;
		ToFileDownloadHandler f;

		public override void Reset()
		{
			url = null;
			storeText = null;
			storeTexture = null;
			storeAudio = null;
			saveInFile = null;
			errorString = null;
			progress = null;
			isDone = null;
            isError = null;
		}

		public override void OnEnter()
		{
			if (string.IsNullOrEmpty (url.Value))
			{
				Finish ();
				return;
			}
				
			if (!storeTexture.IsNone || !storeSprite.IsNone)
			{
				uwr = UnityWebRequestTexture.GetTexture (url.Value);
			}
			else if (!storeAudio.IsNone){
			    uwr = UnityWebRequestMultimedia.GetAudioClip(url.Value, AudioType.UNKNOWN);
			}
			else if (!saveInFile.IsNone)
			{
				uwr = new UnityWebRequest(url.Value);
                try
                {
                    f = new ToFileDownloadHandler(new byte[64 * 1024], saveInFile.Value);
                }catch(Exception e)
                {
                    errorString.Value = e.Message;
                    Fsm.Event(isError);
                    Finish();
                    return;
                }

				uwr.downloadHandler = f;
				
			}else {
				uwr = new UnityWebRequest(url.Value);
				d = new DownloadHandlerBuffer();
				uwr.downloadHandler = d;
			}

			uwr.SendWebRequest();

		}


	
		public override void OnUpdate()
		{
			if (uwr == null)
			{
				errorString.Value = "Unity Web Request is Null!";
                Fsm.Event(isError);
                Finish();
				return;
			}

			errorString.Value = uwr.error;

			if (!string.IsNullOrEmpty(uwr.error))
			{
				Finish();
				Fsm.Event(isError);
				return;
			}


			progress.Value = uwr.downloadProgress;

			//if (progress.Value.Equals(1f))
			if (uwr.isDone)
			{
				if (!storeText.IsNone)
				{
					storeText.Value = uwr.downloadHandler.text;
				}

				if (!storeTexture.IsNone || !storeSprite.IsNone)
				{
					Texture2D _tex = ((DownloadHandlerTexture)uwr.downloadHandler).texture as Texture2D;
					if (!storeTexture.IsNone)
					    storeTexture.Value = _tex;
					else
					{
					    Rect rec = new Rect(0, 0, _tex.width, _tex.height);
  					    storeSprite.Value =	Sprite.Create(_tex, rec, new Vector2(.5f, .5f));
					}
				}
				else if (!storeAudio.IsNone)
				{
					storeAudio.Value = DownloadHandlerAudioClip.GetContent(uwr);
				}

				errorString.Value = uwr.error;

				Fsm.Event(string.IsNullOrEmpty(errorString.Value) ? isDone : isError);

				if (uwr.downloadHandler != null)
				{
					uwr.downloadHandler.Dispose ();
				}

				Finish();
			}
		}

        public override string ErrorCheck()
        {
			int nCount = 0;
			if (!storeText.IsNone)
				nCount += 1;
			if (!storeTexture.IsNone)
				nCount += 1;
		    if (!storeSprite.IsNone)
				nCount += 1;
			if (!storeAudio.IsNone)
				nCount += 1;
			if (!saveInFile.IsNone)
				nCount += 1;
				
			if (nCount == 0)
		        return "Setup at least storeText, storeTexture, storeAudio or saveInFile property to save the data";
			else if (nCount > 1)
				return "You can only select one type of result";
			/*	
            if (storeText.IsNone && storeTexture.IsNone && saveInFile.IsNone)
            {
                return "Setup at least storeText, storeTexture, storeAudio or saveInFile property to save the data";
            }
			if (storeText.IsNone && ( !storeTexture.IsNone && !saveInFile.IsNone))
            {
                return "You can only select one type of result";
            }

            if (storeTexture.IsNone && (!storeText.IsNone && !saveInFile.IsNone))
            {
                return "You can only select one type of result";
            }

            if (saveInFile.IsNone && (!storeTexture.IsNone && !storeText.IsNone))
            {
                return "You can only select one type of result";
            }*/

            return string.Empty;
        }

    }

	public class ToFileDownloadHandler : DownloadHandlerScript
	{
		private int expected = -1;
		private int received = 0;
		private string filepath;
		private FileStream fileStream;
		private bool canceled = false;

		public ToFileDownloadHandler(byte[] buffer, string filepath): base(buffer)
		{
			this.filepath = filepath;
			fileStream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
		}

		protected override byte[] GetData() { return null; }

		protected override bool ReceiveData(byte[] data, int dataLength)
		{
			if (data == null || data.Length < 1)
			{
				return false;
			}
			received += dataLength;
			if (!canceled) fileStream.Write(data, 0, dataLength);
			return true;
		}

		protected override float GetProgress()
		{
			if (expected < 0) return 0;
			return (float)received / expected;
		}

		protected override void CompleteContent()
		{
			fileStream.Close();
		}

		protected override void ReceiveContentLength(int contentLength)
		{
			expected = contentLength;
		}

		public void Cancel()
		{
			canceled = true;
			fileStream.Close();
			File.Delete(filepath);
		}
	}


}

