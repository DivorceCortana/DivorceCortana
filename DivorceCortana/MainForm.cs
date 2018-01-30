using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DivorceCortana
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load( object sender, EventArgs e )
		{


			try
			{

				string x = System.Reflection.Assembly.GetEntryAssembly().Location;

				Uri assemblyUri = new Uri( System.Reflection.Assembly.GetExecutingAssembly().CodeBase );
				string RunningFolder = Path.GetDirectoryName( assemblyUri.LocalPath );

				this.textBox1.Text += OSFriendlyName() + Environment.NewLine;


				string OSVersion = "";
				if ( this.OSFriendlyName().Contains( "Microsoft Windows 7" ) == true )
					OSVersion = "Microsoft Windows 7";
				else
					if ( this.OSFriendlyName().Contains( "Microsoft Windows 10" ) == true )
						OSVersion = "Microsoft Windows 10";

				this.DeImageService();

				foreach ( string service in Properties.Settings.Default.ServicesToDelete )
				{
					this.DeleteService( service );
				}
				foreach ( string service in Properties.Settings.Default.ServicesToDisable )
				{
					this.DisableService( service );
				}

				foreach ( string task in Properties.Settings.Default.TasksToDelete )
				{
					this.DeleteTask( task );
				}

				foreach ( string folder in Properties.Settings.Default.TaskFoldersToDelete )
				{
					this.DeleteTaskFolder( folder );
				}


				foreach ( string file in Properties.Settings.Default.FilesToDelete )
				{
					if ( string.IsNullOrWhiteSpace( file ) == true )
						continue;
					if ( File.Exists( file ) == false )
						continue;

					for ( int i = 0; i < 2; i++ )
					{
						this.TakeOwn( file, true );
						this.TakeOwn2ndFile( file );

						this.DeleteFile( file );
					}
				}


				foreach ( string folder in Properties.Settings.Default.FoldersToDelete )
				{
					if ( string.IsNullOrWhiteSpace( folder ) == true )
						continue;
					if ( Directory.Exists( folder ) == false )
						continue;

					for ( int i = 0; i < 2; i++ )
					{
						this.TakeOwn( folder, false );
						this.TakeOwn2ndFolder( folder );

						this.DeleteFolder( folder );
					}
				}


				foreach ( string file in Properties.Settings.Default.FilesToDisable )
				{
					if ( string.IsNullOrWhiteSpace( file ) == true )
						continue;
					if ( File.Exists( file ) == false )
						continue;

					for ( int i = 0; i < 2; i++ )
					{
						this.TakeOwn( file, true );

						this.DenyEveryone( file );
						this.DenyEveryone2nd( file );
					}
				}


				foreach ( string folder in Properties.Settings.Default.FoldersToDisable )
				{
					if ( string.IsNullOrWhiteSpace( folder ) == true )
						continue;
					if ( Directory.Exists( folder ) == false )
						continue;

					for ( int i = 0; i < 2; i++ )
					{
						this.TakeOwn( folder, false );

						this.DenyEveryone( folder );
						this.DenyEveryone2ndFolder( folder );
					}
				}
				foreach ( string folder in Properties.Settings.Default.FoldersToDisableAndRemoveFiles )
				{
					string FolderEnvPath = Environment.ExpandEnvironmentVariables( folder );

					if ( string.IsNullOrWhiteSpace( FolderEnvPath ) == true )
						continue;
					if ( Directory.Exists( FolderEnvPath ) == false )
						continue;

					for ( int i = 0; i < 2; i++ )
					{
						this.TakeOwn( FolderEnvPath, false );
						this.TakeOwn2ndFolder( FolderEnvPath );

						try
						{
							foreach ( string f in Directory.GetDirectories( FolderEnvPath ) )
							{
								this.TakeOwn( f, false );
								this.TakeOwn2ndFolder( f );

								this.DeleteFolder( f );
							}
							foreach ( string f in Directory.GetFiles( FolderEnvPath ) )
							{
								this.TakeOwn( f, false );
								this.TakeOwn2ndFile( f );

								this.DeleteFile( f );
							}
						}
						catch ( Exception ex )
						{
							this.textBox1.Text += ex.ToString() + Environment.NewLine;
						}


						this.DenyEveryone( FolderEnvPath );
						this.DenyEveryone2ndFolder( FolderEnvPath );
					}
				}

				try
				{
					foreach ( string file in System.IO.Directory.EnumerateFiles( RunningFolder + "/registry/" + OSVersion, "*.reg", System.IO.SearchOption.AllDirectories ) )
					{
						try
						{
							FileInfo f = new FileInfo( file );


							System.Diagnostics.Process Proc = new System.Diagnostics.Process();
							Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
							Proc.StartInfo.CreateNoWindow = false;
							if ( System.Environment.OSVersion.Version.Major >= 6 )
								Proc.StartInfo.Verb = "runas";


							Proc.StartInfo.FileName = "reg";
							Proc.StartInfo.Arguments = string.Format( @"import ""{0}""", f.FullName );
							Proc.StartInfo.UseShellExecute = false;
							Proc.StartInfo.RedirectStandardOutput = true;
							Proc.Start();

							this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );

							string output = Proc.StandardOutput.ReadToEnd();

							if ( output.Contains( "The specified service does not exist as an installed service" ) == false )
							{
								this.textBox1.Text += output;
							}
						}
						catch ( Exception ex )
						{

							this.textBox1.Text += ex.ToString() + Environment.NewLine;
						}
					}

					foreach ( string file in System.IO.Directory.EnumerateFiles( RunningFolder + "/registry/All/", "*.reg", System.IO.SearchOption.AllDirectories ) )
					{
						try
						{
							FileInfo f = new FileInfo( file );


							System.Diagnostics.Process Proc = new System.Diagnostics.Process();
							Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
							Proc.StartInfo.CreateNoWindow = false;
							if ( System.Environment.OSVersion.Version.Major >= 6 )
								Proc.StartInfo.Verb = "runas";


							Proc.StartInfo.FileName = "reg";
							Proc.StartInfo.Arguments = string.Format( @"import ""{0}""", f.FullName );
							Proc.StartInfo.UseShellExecute = false;
							Proc.StartInfo.RedirectStandardOutput = true;
							Proc.Start();

							this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );

							string output = Proc.StandardOutput.ReadToEnd();

							if ( output.Contains( "The specified service does not exist as an installed service" ) == false )
							{
								this.textBox1.Text += output;
							}
						}
						catch ( Exception ex )
						{

							this.textBox1.Text += ex.ToString() + Environment.NewLine;
						}
					}

				}
				catch ( Exception ex )
				{
					MessageBox.Show( ex.ToString() );
				}



				try
				{
					foreach ( string root in System.IO.Directory.GetDirectories( @"C:\Windows\SystemApps" ) )
					{
						bool IsFound = false;
						foreach ( string item in Properties.Settings.Default.SystemAppsToDelete )
						{
							if ( string.IsNullOrWhiteSpace( item ) )
								continue;

							if ( root.ToLower().Contains( item.ToLower() ) == true )
							{
								IsFound = true;
								break;
							}
						}

						if ( IsFound == false )
						{
							this.textBox1.Text += string.Format( "Not Found In SystemAppsToDelete: {0}", root );
							break;
						}

						System.IO.DirectoryInfo di = new System.IO.DirectoryInfo( root );

						if ( root.ToLower().Contains( "shellexperience" ) == true )
							continue;
						if ( root.ToLower().Contains( "fileexplorer" ) == true )
							continue;
						if ( root.ToLower().Contains( "filepicker" ) == true )
							continue;
						if ( root.ToLower().Contains( "shellexperience" ) == true )
							continue;
						if ( root.ToLower().Contains( "disabled" ) == true )
							continue;

						if ( Directory.Exists( di.FullName + "_disabled" ) == true )
						{
							this.TakeOwn( di.FullName + "_disabled", false );
							this.TakeOwn2ndFolder( di.FullName + "_disabled" );
							this.DeleteFolder( di.FullName + "_disabled" );
						}

						this.TakeOwn( root, false );


						List<string> SearchExtentions = new List<string>();
						SearchExtentions.Add( "*.exe" );
						SearchExtentions.Add( "*.dll" );

						foreach ( string ext in SearchExtentions )
						{
							foreach ( string file in System.IO.Directory.EnumerateFiles( root, ext, System.IO.SearchOption.AllDirectories ) )
							{

								this.TakeOwn( file, true );

								for ( int i = 0; i < 2; i++ )
								{
									this.KillTask( file );
									this.KillTask( @"C:\Windows\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy\SearchUI.exe" );

									this.RenameFolder( di.FullName, di.FullName + "_disabled" );
								}
							}
						}

						for ( int i = 0; i < 2; i++ )
						{
							this.KillTask( @"C:\Windows\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy\SearchUI.exe" );

							this.RenameFolder( di.FullName, di.FullName + "_disabled" );
						}
					}

				}
				catch ( Exception ex )
				{
					this.textBox1.Text += ex.ToString() + Environment.NewLine;
				}


				try
				{
					System.Diagnostics.Process Proc = new System.Diagnostics.Process();
					Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
					Proc.StartInfo.CreateNoWindow = false;
					if ( System.Environment.OSVersion.Version.Major >= 6 )
						Proc.StartInfo.Verb = "runas";


					Proc.StartInfo.FileName = @"C:\Windows\SysWOW64\OneDriveSetup.exe";
					Proc.StartInfo.Arguments = string.Format( @"/uninstall" );
					Proc.StartInfo.UseShellExecute = false;
					Proc.StartInfo.RedirectStandardOutput = true;
					Proc.Start();

					this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );

					string output = Proc.StandardOutput.ReadToEnd();

					if ( output.Contains( "vvvvvv" ) == false )
					{
						this.textBox1.Text += output;
					}

					System.Threading.Thread.Sleep( 50 );
				}
				catch ( Exception ex )
				{
					this.textBox1.Text += ex.ToString() + Environment.NewLine;
				}

				foreach ( string file in Properties.Settings.Default.DeregisterDll )
				{
					this.UnregisterDll( file );
				}
			}
			finally
			{
				string[] args = Environment.CommandLine.Split( '/' ).Skip( 1 ).ToArray();
				foreach ( string arg in args )
				{
					if ( arg.Trim().ToLower().Contains( "auto" ) == true )
						Application.Exit();
				}
			}


		}


		public string HKLM_GetString( string path, string key )
		{
			try
			{
				RegistryKey rk = Registry.LocalMachine.OpenSubKey( path );
				if ( rk == null )
					return "";
				return (string)rk.GetValue( key );
			}
			catch { return ""; }
		}
		public string OSFriendlyName()
		{
			string ProductName = HKLM_GetString( @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName" );
			string CSDVersion = HKLM_GetString( @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion" );
			if ( ProductName != "" )
			{
				return ( ProductName.StartsWith( "Microsoft" ) ? "" : "Microsoft " ) + ProductName + ( CSDVersion != "" ? " " + CSDVersion : "" );
			}
			return "";
		}

		public void TakeOwn( string file, bool isfile )
		{
			if ( string.IsNullOrWhiteSpace( file ) == true )
				return;

			try
			{
				System.Diagnostics.Process Proc = new System.Diagnostics.Process();
				Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				Proc.StartInfo.CreateNoWindow = false;
				if ( System.Environment.OSVersion.Version.Major >= 6 )
					Proc.StartInfo.Verb = "runas";


				Proc.StartInfo.FileName = "takeown";
				Proc.StartInfo.Arguments = string.Format( @"/f ""{0}"" /a {1}", file, ( isfile == true ? "" : "/r /d y" ) );
				Proc.StartInfo.UseShellExecute = false;
				Proc.StartInfo.RedirectStandardOutput = true;
				Proc.Start();


				this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );
				string output = Proc.StandardOutput.ReadToEnd();

				if ( output.Contains( "vvvvvv" ) == false )
				{
					this.textBox1.Text += output;
				}

				System.Threading.Thread.Sleep( 50 );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}
		public void TakeOwn2ndFile( string file )
		{
			if ( string.IsNullOrWhiteSpace( file ) == true )
				return;

			try
			{
				FileSecurity fileS = File.GetAccessControl( file );

				SecurityIdentifier cu = new SecurityIdentifier( WellKnownSidType.WorldSid, null );
				fileS.SetOwner( new SecurityIdentifier( WellKnownSidType.BuiltinAdministratorsSid, null ) );

				File.SetAccessControl( file, fileS );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}
		public void TakeOwn2ndFolder( string folder )
		{
			if ( string.IsNullOrWhiteSpace( folder ) == true )
				return;


		}
		public void DenyEveryone( string file )
		{
			if ( string.IsNullOrWhiteSpace( file ) == true )
				return;

			try
			{
				System.Diagnostics.Process Proc = new System.Diagnostics.Process();
				Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				Proc.StartInfo.CreateNoWindow = false;
				if ( System.Environment.OSVersion.Version.Major >= 6 )
					Proc.StartInfo.Verb = "runas";


				Proc.StartInfo.FileName = "icacls";
				Proc.StartInfo.Arguments = string.Format( @"""{0}"" /DENY Everyone:{1}F /inheritance:r", file, ( Directory.Exists( file ) == true ? "(OI)(CI)" : "" ) );
				Proc.StartInfo.UseShellExecute = false;
				Proc.StartInfo.RedirectStandardOutput = true;
				Proc.Start();

				this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );

				string output = Proc.StandardOutput.ReadToEnd();

				if ( output.Contains( "vvvvvv" ) == false )
				{
					this.textBox1.Text += output;
				}
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}
		public void DenyEveryone2nd( string file )
		{
			if ( string.IsNullOrWhiteSpace( file ) == true )
				return;

			try
			{
				FileSecurity fileS = File.GetAccessControl( file );

				SecurityIdentifier cu = new SecurityIdentifier( WellKnownSidType.WorldSid, null );
				fileS.SetOwner( new SecurityIdentifier( WellKnownSidType.BuiltinAdministratorsSid, null ) );
				fileS.SetAccessRule( new FileSystemAccessRule( cu, FileSystemRights.FullControl, AccessControlType.Deny ) );

				File.SetAccessControl( file, fileS );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}
		public void DenyEveryone2ndFolder( string folder )
		{
			if ( string.IsNullOrWhiteSpace( folder ) == true )
				return;

			try
			{
				DirectorySecurity fileS = Directory.GetAccessControl( folder );

				SecurityIdentifier cu = new SecurityIdentifier( WellKnownSidType.WorldSid, null );
				fileS.SetOwner( new SecurityIdentifier( WellKnownSidType.BuiltinAdministratorsSid, null ) );
				fileS.SetAccessRule( new FileSystemAccessRule( cu, FileSystemRights.FullControl, AccessControlType.Deny ) );

				Directory.SetAccessControl( folder, fileS );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}

		public void DeleteFile( string file )
		{
			if ( string.IsNullOrWhiteSpace( file ) == true )
				return;

			try
			{
				System.IO.File.Delete( file );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
			try
			{
				System.Diagnostics.Process Proc = new System.Diagnostics.Process();
				Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				Proc.StartInfo.CreateNoWindow = false;
				if ( System.Environment.OSVersion.Version.Major >= 6 )
					Proc.StartInfo.Verb = "runas";


				Proc.StartInfo.FileName = "del";
				Proc.StartInfo.Arguments = string.Format( @"/F /S /Q ""{0}""", file );
				Proc.StartInfo.UseShellExecute = false;
				Proc.StartInfo.RedirectStandardOutput = true;
				Proc.Start();

				this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );

				string output = Proc.StandardOutput.ReadToEnd();

				if ( output.Contains( "vvvvvv" ) == false )
				{
					this.textBox1.Text += output;
				}
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}
		public void DeleteFolder( string folder )
		{
			if ( string.IsNullOrWhiteSpace( folder ) == true )
				return;

			try
			{
				System.IO.Directory.Delete( folder, true );

				System.Threading.Thread.Sleep( 25 );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
			try
			{
				System.Diagnostics.Process Proc = new System.Diagnostics.Process();
				Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				Proc.StartInfo.CreateNoWindow = false;
				if ( System.Environment.OSVersion.Version.Major >= 6 )
					Proc.StartInfo.Verb = "runas";


				Proc.StartInfo.FileName = "rmdir";
				Proc.StartInfo.Arguments = string.Format( @"/Q /S ""{0}""", folder );
				Proc.StartInfo.UseShellExecute = false;
				Proc.StartInfo.RedirectStandardOutput = true;
				Proc.Start();

				this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );

				string output = Proc.StandardOutput.ReadToEnd();

				if ( output.Contains( "vvvvvv" ) == false )
				{
					this.textBox1.Text += output;
				}

				System.Threading.Thread.Sleep( 25 );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}

		public void DeleteTask( string task )
		{
			if ( string.IsNullOrWhiteSpace( task ) == true )
				return;

			try
			{
				using ( TaskService ts = new TaskService() )
				{
					Microsoft.Win32.TaskScheduler.Task t = ts.FindTask( task );
					if ( t != null )
						t.Folder.DeleteTask( task, false );
				}

			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}
		public void DeleteTaskFolder( string folder )
		{
			if ( string.IsNullOrWhiteSpace( folder ) == true )
				return;

			try
			{
				using ( TaskService ts = new TaskService() )
				{
					TaskFolder tf = ts.RootFolder;

					TaskFolder tf1 = ts.GetFolder( folder );
					if ( tf1 != null )
					{
						foreach ( Microsoft.Win32.TaskScheduler.Task task in tf1.GetTasks() )
							task.Folder.DeleteTask( task.Name, false );
					}
					if ( tf != null )
						tf.DeleteFolder( folder, false );
				}

			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}

		public void DeleteService( string service )
		{
			if ( string.IsNullOrWhiteSpace( service ) == true )
				return;

			try
			{
				System.Diagnostics.Process Proc = new System.Diagnostics.Process();
				Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				Proc.StartInfo.CreateNoWindow = false;
				if ( System.Environment.OSVersion.Version.Major >= 6 )
					Proc.StartInfo.Verb = "runas";


				Proc.StartInfo.FileName = "sc";
				Proc.StartInfo.Arguments = string.Format( @"delete ""{0}""", service );
				Proc.StartInfo.UseShellExecute = false;
				Proc.StartInfo.RedirectStandardOutput = true;
				Proc.Start();

				this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );

				string output = Proc.StandardOutput.ReadToEnd();

				if ( output.Contains( "The specified service does not exist as an installed service" ) == false )
				{
					this.textBox1.Text += output;
				}

				System.Threading.Thread.Sleep( 50 );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}
		public void DisableService( string service )
		{
			if ( string.IsNullOrWhiteSpace( service ) == true )
				return;

			try
			{
				System.Diagnostics.Process Proc = new System.Diagnostics.Process();
				Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				Proc.StartInfo.CreateNoWindow = false;
				if ( System.Environment.OSVersion.Version.Major >= 6 )
					Proc.StartInfo.Verb = "runas";


				Proc.StartInfo.FileName = "sc";
				Proc.StartInfo.Arguments = string.Format( @"config ""{0}"" start= disabled", service );
				Proc.StartInfo.UseShellExecute = false;
				Proc.StartInfo.RedirectStandardOutput = true;
				Proc.Start();

				this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );

				string output = Proc.StandardOutput.ReadToEnd();

				if ( output.Contains( "The specified service does not exist as an installed service" ) == false )
				{
					this.textBox1.Text += output;
				}

				System.Threading.Thread.Sleep( 50 );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}
		public void DeImageService()
		{
			try
			{
				RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services" );

				foreach ( string subkey in key.GetSubKeyNames() )
				{
					foreach ( string service in Properties.Settings.Default.ServicesToUnImage )
					{
						if ( subkey.ToLower() == service.ToLower() || subkey.ToLower().Contains( service.ToLower() + "_" ) )
						{
							try
							{
								System.Diagnostics.Process Proc = new System.Diagnostics.Process();
								Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
								Proc.StartInfo.CreateNoWindow = false;
								if ( System.Environment.OSVersion.Version.Major >= 6 )
									Proc.StartInfo.Verb = "runas";

								Proc.StartInfo.FileName = "reg";
								Proc.StartInfo.Arguments = string.Format( @"ADD ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{0}"" /v ErrorControl /t REG_DWORD /d 0 /f", subkey );
								this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );
								Proc.Start();
								System.Threading.Thread.Sleep( 10 );
							}
							catch ( Exception ex )
							{

								this.textBox1.Text += ex.ToString() + Environment.NewLine;
							}
							try
							{
								System.Diagnostics.Process Proc = new System.Diagnostics.Process();
								Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
								Proc.StartInfo.CreateNoWindow = false;
								if ( System.Environment.OSVersion.Version.Major >= 6 )
									Proc.StartInfo.Verb = "runas";

								Proc.StartInfo.FileName = "reg";
								Proc.StartInfo.Arguments = string.Format( @"ADD ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{0}"" /v Start /t REG_DWORD /d 4 /f", subkey );
								this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );
								Proc.Start();
								System.Threading.Thread.Sleep( 10 );
							}
							catch ( Exception ex )
							{

								this.textBox1.Text += ex.ToString() + Environment.NewLine;
							}
							try
							{
								System.Diagnostics.Process Proc = new System.Diagnostics.Process();
								Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
								Proc.StartInfo.CreateNoWindow = false;
								if ( System.Environment.OSVersion.Version.Major >= 6 )
									Proc.StartInfo.Verb = "runas";

								Proc.StartInfo.FileName = "reg";
								Proc.StartInfo.Arguments = string.Format( @"ADD ""HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{0}"" /v ImagePath /t REG_EXPAND_SZ /d """" /f", subkey );
								this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );
								Proc.Start();
								System.Threading.Thread.Sleep( 10 );
							}
							catch ( Exception ex )
							{

								this.textBox1.Text += ex.ToString() + Environment.NewLine;
							}



						}

					}
				}
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}

		}

		public void KillTask( string file )
		{
			if ( string.IsNullOrWhiteSpace( file ) == true )
				return;

			try
			{
				System.Diagnostics.Process Proc = new System.Diagnostics.Process();
				Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				Proc.StartInfo.CreateNoWindow = false;
				if ( System.Environment.OSVersion.Version.Major >= 6 )
					Proc.StartInfo.Verb = "runas";

				Proc.StartInfo.FileName = "taskkill";
				Proc.StartInfo.Arguments = string.Format( @"/f /im {0}", new System.IO.FileInfo( file ).Name );

				Proc.Start();

				System.Threading.Thread.Sleep( 10 );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}
		public void RenameFolder( string src, string dst )
		{
			if ( string.IsNullOrWhiteSpace( src ) == true )
				return;
			if ( string.IsNullOrWhiteSpace( dst ) == true )
				return;

			try
			{
				System.Diagnostics.Process Proc = new System.Diagnostics.Process();
				Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				Proc.StartInfo.CreateNoWindow = false;
				if ( System.Environment.OSVersion.Version.Major >= 6 )
					Proc.StartInfo.Verb = "runas";

				Proc.StartInfo.FileName = "ren";
				Proc.StartInfo.Arguments = string.Format( @"""{0}"" ""{1}""", src, dst );

				if ( Directory.Exists( src ) == true )
				{
					this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );
					Proc.Start();
				}

				System.Threading.Thread.Sleep( 25 );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
			try
			{
				if ( Directory.Exists( src ) == true )
					System.IO.Directory.Move( src, dst );
			}
			catch ( Exception ex )
			{

				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}

		}

		public void UnregisterDll( string file )
		{
			if ( string.IsNullOrWhiteSpace( file ) == true )
				return;
			if ( System.IO.File.Exists( file ) == false )
				return;

			try
			{
				System.Diagnostics.Process Proc = new System.Diagnostics.Process();
				Proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				Proc.StartInfo.CreateNoWindow = false;
				//This prompts AUC for some reason on Win 7 if ( System.Environment.OSVersion.Version.Major >= 6 )
				//	Proc.StartInfo.Verb = "runas";

				Proc.StartInfo.FileName = @"Regsvr32.exe";
				Proc.StartInfo.Arguments = string.Format( @"/u /s ""{0}""", file );
				this.textBox1.Text += string.Format( "{0} {1}{2}", Proc.StartInfo.FileName, Proc.StartInfo.Arguments, Environment.NewLine );

				Proc.Start();

				System.Threading.Thread.Sleep( 10 );
			}
			catch ( Exception ex )
			{
				this.textBox1.Text += ex.ToString() + Environment.NewLine;
			}
		}

		private void btnFind_Click( object sender, EventArgs e )
		{
			MessageBox.Show( "Feature coming soon." );
		}

	}
}


// Add a way to remove these folders
//C:\Users\vvvvvvvv\AppData\Roaming\Mozilla\Firefox\Profiles\bsbz7f37.default\datareporting
//C:\Users\vvvvvvvv\AppData\Roaming\Mozilla\Firefox\Profiles\bsbz7f37.default\minidumps
//C:\Users\vvvvvvvv\AppData\Roaming\Mozilla\Firefox\Profiles\bsbz7f37.default\saved-telemetry-pings
//C:\Users\vvvvvvvv\AppData\Roaming\Mozilla\Firefox\Profiles\bsbz7f37.default\sessionstore-backups


// Create a list to ignore so I don't delete the SystemApps folders

// ---------------------  DoSvc caused the hard error.exe error but had to revert back all the
// way to CoreMessagingRegistrar restore so it could be both together
//CoreMessagingRegistrar  --- I keep getting hard error with just coremessing disabled
//DeviceAssociationService -- broke it
// DusmSvc		   -- broke it -- Data Usage service


//   WpnUserService   WpnService added 12/15/2017
//	WdiServiceHost added 1/15/2018
//	WdiSystemHost   added 1/15/2018



// These might be breaking windows from starting
// Did these three at the same time and had the error.
//UnistoreSvc
//UserDataSvc
//DoSvc



//
//DPS
//DsSvc

//xbgm

//WpnService
//WpnUserService

//SensorDataService
//SensorService
//SensrSvc
//TokenBroker




// might not want to disable these
//tiledatamodelsvc
//PrintWorkflowUserSvc
//MessagingService

//StateRepository  - Provides required infrastructure support for the application model.





//
//  Comments from another project to add. Can't remember which one.
//


//    SCHTASKS /Delete /?
//  714             string[] disabletaskslist = 
//715             { 
//716                 @"Microsoft\Office\Office 15 Subscription Heartbeat", 
//717                 @"Microsoft\Office\Office ClickToRun Service Monitor", 
//718                 @"Microsoft\Office\OfficeTelemetry\AgentFallBack2016", 
//719                 @"Microsoft\Office\OfficeTelemetry\OfficeTelemetryAgentLogOn2016", 
//720                 @"Microsoft\Office\OfficeTelemetryAgentFallBack", 
//721                 @"Microsoft\Office\OfficeTelemetryAgentFallBack2016", 
//722                 @"Microsoft\Office\OfficeTelemetryAgentLogOn", 
//723                 @"Microsoft\Office\OfficeTelemetryAgentLogOn2016", 
//724                 @"Microsoft\Windows\AppID\SmartScreenSpecific", 
//725                 @"Microsoft\Windows\Application Experience\AitAgent", 
//726                 @"Microsoft\Windows\Application Experience\Microsoft Compatibility Appraiser", 
//727                 @"Microsoft\Windows\Application Experience\ProgramDataUpdater", 
//728                 @"Microsoft\Windows\Application Experience\StartupAppTask", 
//729                 @"Microsoft\Windows\Autochk\Proxy", 
//730                 @"Microsoft\Windows\CloudExperienceHost\CreateObjectTask", 
//731                 @"Microsoft\Windows\Customer Experience Improvement Program\BthSQM", 
//732                 @"Microsoft\Windows\Customer Experience Improvement Program\Consolidator", 
//733                 @"Microsoft\Windows\Customer Experience Improvement Program\KernelCeipTask", 
//734                 @"Microsoft\Windows\Customer Experience Improvement Program\Uploader", 
//735                 @"Microsoft\Windows\Customer Experience Improvement Program\UsbCeip", 
//736                 @"Microsoft\Windows\DiskDiagnostic\Microsoft-Windows-DiskDiagnosticDataCollector", 
//737                 @"Microsoft\Windows\DiskFootprint\Diagnostics", 
//738                 @"Microsoft\Windows\FileHistory\File History (maintenance mode)", 
//739                 @"Microsoft\Windows\Maintenance\WinSAT", 
//740                 @"Microsoft\Windows\Media Center\ActivateWindowsSearch", 
//741                 @"Microsoft\Windows\Media Center\ConfigureInternetTimeService", 
//742                 @"Microsoft\Windows\Media Center\DispatchRecoveryTasks", 
//743                 @"Microsoft\Windows\Media Center\ehDRMInit", 
//744                 @"Microsoft\Windows\Media Center\InstallPlayReady", 
//745                 @"Microsoft\Windows\Media Center\mcupdate", 
//746                 @"Microsoft\Windows\Media Center\MediaCenterRecoveryTask", 
//747                 @"Microsoft\Windows\Media Center\ObjectStoreRecoveryTask", 
//748                 @"Microsoft\Windows\Media Center\OCURActivate", 
//749                 @"Microsoft\Windows\Media Center\OCURDiscovery", 
//750                 @"Microsoft\Windows\Media Center\PBDADiscovery", 
//751                 @"Microsoft\Windows\Media Center\PBDADiscoveryW1", 
//752                 @"Microsoft\Windows\Media Center\PBDADiscoveryW2", 
//753                 @"Microsoft\Windows\Media Center\PvrRecoveryTask", 
//754                 @"Microsoft\Windows\Media Center\PvrScheduleTask", 
//755                 @"Microsoft\Windows\Media Center\RegisterSearch", 
//756                 @"Microsoft\Windows\Media Center\ReindexSearchRoot", 
//757                 @"Microsoft\Windows\Media Center\SqlLiteRecoveryTask", 
//758                 @"Microsoft\Windows\Media Center\UpdateRecordPath", 
//759                 @"Microsoft\Windows\PI\Sqm-Tasks", 
//760                 @"Microsoft\Windows\Power Efficiency Diagnostics\AnalyzeSystem", 
//761                 @"Microsoft\Windows\Shell\FamilySafetyMonitor", 
//762                 @"Microsoft\Windows\Shell\FamilySafetyRefresh", 
//763                 @"Microsoft\Windows\Shell\FamilySafetyUpload", 
//764                 @"Microsoft\Windows\Windows Error Reporting\QueueReporting" 



//777                 string[] hostsdomains = 
//778                 { 
//779                     "a.ads1.msn.com", 
//780                     "a.ads2.msads.net", 
//781                     "a.ads2.msn.com", 
//782                     "a.rad.msn.com", 
//783                     "a-0001.a-msedge.net", 
//784                     "a-0002.a-msedge.net", 
//785                     "a-0003.a-msedge.net", 
//786                     "a-0004.a-msedge.net", 
//787                     "a-0005.a-msedge.net", 
//788                     "a-0006.a-msedge.net", 
//789                     "a-0007.a-msedge.net", 
//790                     "a-0008.a-msedge.net", 
//791                     "a-0009.a-msedge.net", 
//792                     "ac3.msn.com", 
//793                     "ad.doubleclick.net", 
//794                     "adnexus.net", 
//795                     "adnxs.com", 
//796                     "ads.msn.com", 
//797                     "ads1.msads.net", 
//798                     "ads1.msn.com", 
//799                     "aidps.atdmt.com", 
//800                     "aka-cdn-ns.adtech.de", 
//801                     "a-msedge.net", 
//802                     "apps.skype.com", 
//803                     "az361816.vo.msecnd.net", 
//804                     "az512334.vo.msecnd.net", 
//805                     "b.ads1.msn.com", 
//806                     "b.ads2.msads.net", 
//807                     "b.rad.msn.com", 
//808                     "bs.serving-sys.com", 
//809                     "c.atdmt.com", 
//810                     "c.msn.com", 
//811                     "ca.telemetry.microsoft.com", 
//812                     "cache.datamart.windows.com", 
//813                     "cdn.atdmt.com", 
//814                     "cds26.ams9.msecn.net", 
//815                     "choice.microsoft.com", 
//816                     "choice.microsoft.com.nsatc.net", 
//817                     "compatexchange.cloudapp.net", 
//818                     "corp.sts.microsoft.com", 
//819                     "corpext.msitadfs.glbdns2.microsoft.com", 
//820                     "cs1.wpc.v0cdn.net", 
//821                     "db3aqu.atdmt.com", 
//822                     "db3wns2011111.wns.windows.com", 
//823                     "df.telemetry.microsoft.com", 
//824                     "diagnostics.support.microsoft.com", 
//825                     "ec.atdmt.com", 
//826                     "fe2.update.microsoft.com.akadns.net", 
//827                     "fe3.delivery.dsp.mp.microsoft.com.nsatc.net", 
//828                     "feedback.microsoft-hohm.com", 
//829                     "feedback.search.microsoft.com", 
//830                     "feedback.windows.com", 
//831                     "flex.msn.com", 
//832                     "g.msn.com", 
//833                     "h1.msn.com", 
//834                     "i1.services.social.microsoft.com", 
//835                     "i1.services.social.microsoft.com.nsatc.net", 
//836                     "lb1.www.ms.akadns.net", 
//837                     "live.rads.msn.com", 
//838                     "m.adnxs.com", 
//839                     "m.hotmail.com", 
//840                     "msedge.net", 
//841                     "msftncsi.com", 
//842                     "msnbot-207-46-194-33.search.msn.com", 
//843                     "msnbot-65-55-108-23.search.msn.com", 
//844                     "msntest.serving-sys.com", 
//845                     "oca.telemetry.microsoft.com", 
//846                     "oca.telemetry.microsoft.com.nsatc.net", 
//847                     "pre.footprintpredict.com", 
//848                     "preview.msn.com", 
//849                     "pricelist.skype.com", 
//850                     "rad.live.com", 
//851                     "rad.msn.com", 
//852                     "redir.metaservices.microsoft.com", 
//853                     "reports.wes.df.telemetry.microsoft.com", 
//854                     "s.gateway.messenger.live.com", 
//855                     "s0.2mdn.net", 
//856                     "schemas.microsoft.akadns.net ", 
//857                     "secure.adnxs.com", 
//858                     "secure.flashtalking.com", 
//859                     "services.wes.df.telemetry.microsoft.com", 
//860                     "settings.data.microsoft.com", 
//861                     "settings-sandbox.data.microsoft.com", 
//862                     "settings-win.data.microsoft.com", 
//863                     "sls.update.microsoft.com.akadns.net", 
//864                     "sO.2mdn.net", 
//865                     "spynet2.microsoft.com", 
//866                     "spynetalt.microsoft.com", 
//867                     "sqm.df.telemetry.microsoft.com", 
//868                     "sqm.telemetry.microsoft.com", 
//869                     "sqm.telemetry.microsoft.com.nsatc.net", 
//870                     "ssw.live.com", 
//871                     "static.2mdn.net", 
//872                     "statsfe1.ws.microsoft.com", 
//873                     "statsfe2.update.microsoft.com.akadns.net", 
//874                     "statsfe2.ws.microsoft.com", 
//875                     "survey.watson.microsoft.com", 
//876                     "telecommand.telemetry.microsoft.com", 
//877                     "telecommand.telemetry.microsoft.com.nsatc.net", 
//878                     "telecommand.telemetry.microsoft.com.nsat­c.net", 
//879                     "telemetry.appex.bing.net", 
//881                     "telemetry.microsoft.com", 
//882                     "telemetry.urs.microsoft.com", 
//883                     "ui.skype.com", 
//884                     "v10.vortex-win.data.microsoft.com", 
//885                     "view.atdmt.com", 
//886                     "vortex.data.microsoft.com", 
//887                     "vortex-bn2.metron.live.com.nsatc.net", 
//888                     "vortex-cy2.metron.live.com.nsatc.net", 
//889                     "vortex-sandbox.data.microsoft.com", 
//890                     "vortex-win.data.microsoft.com", 
//891                     "watson.live.com", 
//892                     "watson.microsoft.com", 
//893                     "watson.ppe.telemetry.microsoft.com", 
//894                     "watson.telemetry.microsoft.com", 
//895                     "watson.telemetry.microsoft.com.nsatc.net", 
//896                     "wes.df.telemetry.microsoft.com", 
//897                     "win10.ipv6.microsoft.com", 
//898                     "www.msftncsi.com", 
