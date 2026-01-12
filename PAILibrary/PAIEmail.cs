using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace PAI.Email
{
	public class PAICredential : ICredentials, ICredentialsByHost
	{
		#region Properties

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		public string Domain { get; set; }

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>
		/// The name of the user.
		/// </value>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets the pass word.
		/// </summary>
		/// <value>
		/// The pass word.
		/// </value>
		public string PassWord { get; set; }

		#endregion Properties

		#region ICredentials Members

		/// <summary>
		/// Returns a <see cref="T:System.Net.NetworkCredential" /> object that is associated with the specified URI, and authentication type.
		/// </summary>
		/// <param name="uri">The <see cref="T:System.Uri" /> that the client is providing authentication for.</param>
		/// <param name="authType">The type of authentication, as defined in the <see cref="P:System.Net.IAuthenticationModule.AuthenticationType" /> property.</param>
		/// <returns>
		/// The <see cref="T:System.Net.NetworkCredential" /> that is associated with the specified URI and authentication type, or, if no credentials are available, null.
		/// </returns>
		public NetworkCredential GetCredential(Uri uri, string authType)
		{
			return new NetworkCredential(UserName, PassWord, Domain);
		}

		#endregion ICredentials Members

		#region ICredentialsByHost Members

		/// <summary>
		/// Returns the credential for the specified host, port, and authentication protocol.
		/// </summary>
		/// <param name="host">The host computer that is authenticating the client.</param>
		/// <param name="port">The port on <paramref name="host " />that the client will communicate with.</param>
		/// <param name="authenticationType">The authentication protocol.</param>
		/// <returns>
		/// A <see cref="T:System.Net.NetworkCredential" /> for the specified host, port, and authentication protocol, or null if there are no credentials available for the specified host, port, and authentication protocol.
		/// </returns>
		public NetworkCredential GetCredential(string host, int port, string authenticationType)
		{
			return new NetworkCredential(UserName, PassWord, Domain);
		}

		#endregion ICredentialsByHost Members
	}
	
	public class PAIEmail
	{
		#region Private Members

		private string _server   = "mail01.pai.local";
		private string _smtpUser = "opconsole";
		private string _smtpPass = "computerroom";
		private string _domain   = "pai";
		private List<string> _recipients = new List<string>();

		#endregion Private Members

		#region Public Properties

		/// <summary>
		/// Gets or sets who the email is from.
		/// </summary>
		/// <value>
		/// From.
		/// </value>
		public string From { get; set; }

		/// <summary>
		/// Gets or sets the recipients <see cref="T:System.Collections.Generic.List" />.
		/// </summary>
		/// <value>
		/// The recipients.
		/// </value>
		public List<string> Recipients { get { return _recipients; } set { _recipients = value; } }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the message body (can be in HTML if ).
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the server credentials.
		/// Create a new <see cref="PAICredential"/> object, set its properties
		/// and set this property to it.
		/// </summary>
		/// <value>
		/// The server credentials.
		/// </value>
		public PAICredential ServerCredentials { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the email body is HTML.
		/// </summary>
		/// <value>
		///   <c>true</c> if the body is in HTML; otherwise, <c>false</c>.
		/// </value>
		public bool IsHTML { get; set; }

		/// <summary>
		/// Gets the last error or "string.Empty" if no error.
		/// </summary>
		/// <value>
		/// The last error.
		/// </value>
		public string LastError { get; private set; }

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Sends the email.
		/// </summary>
		/// <returns>True on success or False if there is an error and LastError is set</returns>
		public bool Send()
		{
			MailMessage msg = new MailMessage();
			SmtpClient smtpSvr = new SmtpClient();
			bool bRet = true;

			try
			{
				// setup our message
				msg.From = new MailAddress(From);
				// add all recipients
				foreach (var s in Recipients)
				{
					msg.To.Add(s);
				}
				msg.Subject = Subject;
				msg.IsBodyHtml = IsHTML;
				msg.Body = Message;

				// setup email server
				smtpSvr.Host = _server;

				// if user supplied credentials use them
				if (ServerCredentials == null || (ServerCredentials.UserName == null || ServerCredentials.PassWord == null))
					smtpSvr.Credentials = new NetworkCredential(_smtpUser, _smtpPass, _domain);
				else
					smtpSvr.Credentials = ServerCredentials;

				// finally send the message
				smtpSvr.Send(msg);
			}
			catch (Exception ex)
			{
				LastError = ex.Message;
				bRet = false;
			}

			return bRet;
		}
		
		#endregion
	}
}
