//Defaults added during class generation
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//additional references added for class

//for SFTP connection:
using Renci.SshNet;

//for message box usage:
using System.Windows;

//Add Nuget package - Ssh.Net by Renci


namespace BoilerPlateDraft.Code_Repo
{
  public class SSHConnection
	{
    //All variables must be assigned, not assigned in the boilerplate template

    //Objects needed for connection to SSH: Host, Port, Username, PrivateKeyFile
    private static string host;
    private static int port;

    //username created for PMP connection on apps hosted on Apps-1 server:
    //APPS1API
    private static string userName;

    //file location of PrivateKeyFile
    //Private key needs to be in RSA format for PMP.
    //Generate using command line - ssh-keygen -m PEM
    //or if needed convert using ssh-keygen -f <fileLocation> -m PEM
    //keygen command specifying RSA and PEM format:
    //ssh-keygen -t rsa -b 4096 -m PEM
    private static string pkFileLocation;

    //client object for SSH connection
    public static SshClient client;

    //boolean to track connection status - initial value is false
    private static bool sshConnected = false;

    //PrivateKeyFile for secure connection over SSH
    private static PrivateKeyFile pkFile;

    
    //function to connect to SSH using declared class variables
    //Values for connection to PMP:
    //host = secpmgr.uwsuper.edu
    //port = 5522
    //username = [API USERNAME]  --Example: "APPS1API"
    //PrivateKeyFile pkFile = new PrivateKeyFile(@"[FILE PATH TO PRIVATE KEY FILE]");
    public static void ConnectSSH()
    {
      try
      {
        pkFile = new PrivateKeyFile(pkFileLocation);
        client = new SshClient(host, port, userName, pkFile);
        client.Connect();
        sshConnected = true;
      }
      catch (Exception ex)
      {
        //MessageBox.Show("Error - " + ex);
      }
    }

    //function to connect to SSH using passed variables
    public static void ConnectSSH(string passedHost, int passedPort, string passedUserName, PrivateKeyFile passedKeyFile)
    {
      try
      {
        client = new SshClient(passedHost, passedPort, passedUserName, passedKeyFile);
        client.Connect();
        sshConnected = true;
      }
      catch (Exception ex)
      {
        //MessageBox.Show("Error - " + ex);
      }
    }

    //function to disconnect from SSH
    public static void DisconnectSSH()
    {
      try
      {
        client.Disconnect();
        sshConnected = false;
      }
      catch (Exception ex)
      {
        //MessageBox.Show("Error - " + ex);
      }
    }

    //returns the connection status of SSH connection
    public static bool IsSSHConnected()
    {
      return sshConnected;
    }

    //retrieve data from SSH - returns in string format
    //must create a command to pass through to the SSH server
    //For retrieving secrets from PMP - Command format is:
    //"RETRIEVE --resource=[RESOURCE NAME] --account=[ACCOUNT NAME]"
    public static string RetrieveSSHData(string command)
    {
      if (IsSSHConnected())
      {
        SshCommand retData = client.CreateCommand(command);
        retData.Execute();
        return retData.Result;
      }
      else
      {
        return "";
      }
    }

  }
}