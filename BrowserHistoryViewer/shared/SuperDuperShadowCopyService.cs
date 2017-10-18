using System;
using System.IO;
using Alphaleonis.Win32.Vss;

namespace BrowserHistoryViewer.shared
{
    public static class SuperDuperShadowCopyService
    {
        public static void CopyFile(string from,string to)
        {
            FileInfo MyFileInfo = new FileInfo(from);
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

            _backup.ExposeSnapshot(MyGuid02, null, VssVolumeSnapshotAttributes.ExposedLocally, "L:");

            // VSS step 8: Copy Files!
            /***********************************
            /* Now we start to copy the files/folders/disk!
            /* Execution time can depend on what we are copying
            /* We can copy one element or several element. 
            /* As long as we are working under the same snapshot, 
            /* the element should be in consist state from the same point-in-time
            /***********************************/
            String sVSSFile1 = from.Replace(_Volume, @"L:\");
            if (File.Exists(sVSSFile1))
                System.IO.File.Copy(sVSSFile1, to + @"\" + System.IO.Path.GetFileName(from), true);


            // VSS step 9: Delete the snapshot (using the Exposed Snapshot name)
            foreach (VssSnapshotProperties prop in _backup.QuerySnapshots())
            {
                if (prop.ExposedName == @"L:\")
                {
                    Console.WriteLine("prop.ExposedNam Found!");
                    _backup.DeleteSnapshot(prop.SnapshotId, true);
                }
            }

            _backup = null;

        }
    }
}
