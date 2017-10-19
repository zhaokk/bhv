using System;
using System.IO;
using Alphaleonis.Win32.Vss;

namespace Ariely_AlphaVSS_Sample02
{
    public class vss
    {
        /// <summary>
        /// This is only first test of using AlphaVSS project. 
        /// Please make sure you read thoroughly and understand the code's comments and adapt it to your system and needs.
        /// We are using constant links for "source files to copy" and for the "destination folder".
        /// In any dynamic application you should get those values from the user.
        /// 
        /// building your project:
        /// -----------------------
        /// 1. Creating New Project
        ///    * Open Visual Studio As Administrator!
        ///    * Chose create new project type "Console application"
        ///    * use Dot.Net 4 framework
        ///    * named the project Ariely_AlphaVSS_Sample01
        /// 2. Add NuGet Package named alphavss (use the search on-line option, and install the package)
        /// 3. Go over the Code's comment and understand it!!! After you are ready you can use it.
        /// 4. add "using Alphaleonis.Win32.Vss;" & "using System.IO;"
        /// </summary>
        /// <param name="args"></param>
        public static void copy(string destination)
        {
            // Getting information on the files to copy (This is not part of the VSS action)
            string userName = Environment.UserName;
            string baseAddress = @"C:\Users\{0}\AppData\Local\Microsoft\Windows\WebCache\WebCacheV01.dat";
            string directory = string.Format(baseAddress, userName);
            String _Source1 = directory;
            String _Destination = destination;
            FileInfo MyFileInfo = new FileInfo(_Source1);
            String _Volume = MyFileInfo.Directory.Root.Name;

            // VSS step 1: Initialize
            IVssImplementation _vssImplementation = VssUtils.LoadImplementation();
            IVssBackupComponents _backup = _vssImplementation.CreateVssBackupComponents();
            _backup.InitializeForBackup(null);

            // VSS step 2: Getting Metadata from all the VSS writers
            _backup.GatherWriterMetadata();

            // VSS step 3: VSS Configuration
            _backup.SetContext(VssVolumeSnapshotAttributes.Persistent | VssVolumeSnapshotAttributes.NoAutoRelease);
            _backup.SetBackupState(false, true, Alphaleonis.Win32.Vss.VssBackupType.Full, false);

            // VSS step 4: Declaring the Volumes that we need to use in this beckup. 
            // The Snapshot is a volume element (Here come the name "Volume Shadow-Copy")
            // For each file that we nee to copy we have to make sure that the propere volume is in the "Snapshot Set"
            Guid MyGuid01 = _backup.StartSnapshotSet();
            Guid MyGuid02 = _backup.AddToSnapshotSet(_Volume, Guid.Empty);

            // VSS step 5: Preparation (Writers & Provaiders need to start preparation)
            _backup.PrepareForBackup();
            // VSS step 6: Create a Snapshot For each volume in the "Snapshot Set"
            _backup.DoSnapshotSet();

            /***********************************
            /* At this point we have a snapshot!
            /* This action should not take more then 60 second, regardless of file or disk size.
            /* THe snapshot is not a backup or any copy!
            /* please more information at http://technet.microsoft.com/en-us/library/ee923636.aspx
            /***********************************/

            // VSS step 7: Expose Snapshot
            /***********************************
            /* Snapshot path look like:
             * \\?\Volume{011682bf-23d7-11e2-93e7-806e6f6e6963}\
             * The build in method System.IO.File.Copy do not work with path like this, 
             * Therefor, we are going to Expose the Snapshot to our application, 
             * by mapping the Snapshot to new virtual volume
             * - Make sure that you are using a volume that is not already exist
             * - This is only for learning purposes. usually we will use the snapshot directly as i show in the next example in the blog 
            /***********************************/
            _backup.ExposeSnapshot(MyGuid02, null, VssVolumeSnapshotAttributes.ExposedLocally, "S:");

            // VSS step 8: Copy Files!
            /***********************************
            /* Now we start to copy the files/folders/disk!
            /* Execution time can depend on what we are copying
            /* We can copy one element or several element. 
            /* As long as we are working under the same snapshot, 
            /* the element should be in consist state from the same point-in-time
            /***********************************/
            String sVSSFile1 = _Source1.Replace(_Volume, @"S:\");
            if (File.Exists(sVSSFile1))
                System.IO.File.Copy(sVSSFile1, _Destination + @"\" + System.IO.Path.GetFileName(_Source1), true);


            // VSS step 9: Delete the snapshot (using the Exposed Snapshot name)
            foreach (VssSnapshotProperties prop in _backup.QuerySnapshots())
            {
                if (prop.ExposedName == @"S:\")
                {
                    Console.WriteLine("prop.ExposedNam Found!");
                    _backup.DeleteSnapshot(prop.SnapshotId, true);
                }
            }

            _backup = null;

           
        }
    }
}
