using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Techendary.Processor;
using Task = System.Threading.Tasks.Task;

namespace extension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(extensionPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class extensionPackage : AsyncPackage, IVsRunningDocTableEvents3
    {
        /// <summary>
        /// extensionPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "e43a6178-2f73-4b56-9b2c-7d0c84e4ff9d";
        private uint rdtCookie;
        private DTE dte;
        private IVsRunningDocumentTable rdt;


        #region Package Members



        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            //showMessage("InitializeAsync: Start");
            await Command1.InitializeAsync(this);
            rdt = (IVsRunningDocumentTable)GetGlobalService(typeof(SVsRunningDocumentTable));
            rdt = (IVsRunningDocumentTable)GetGlobalService(typeof(SVsRunningDocumentTable));
            rdt.AdviseRunningDocTableEvents(this, out rdtCookie);
            dte = (DTE)await GetServiceAsync(typeof(DTE));
            //showMessage("InitializeAsync: End");
            //var project = dte.Solution.Projects.Item(1);
            //Console.WriteLine(project.Name);


        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            SaveProcessedFile(docCookie, "OnAfterSave");
            return VSConstants.S_OK;
        }

        private async void SaveProcessedFile(uint docCookie, string suffix)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            rdt.GetDocumentInfo(docCookie, out uint flags, out uint readLocks, out uint editLocks, out string strDocument, out IVsHierarchy ppHier, out uint pitemid, out IntPtr ppunkDocData);
            //showMessage(strDocument);
            var file = new FileInfo(strDocument);
            //var fileNameWithoutExtension = file.Name.Replace(file.Extension, "");
            var processedXml = Processor.processXml(strDocument);
            var folder = Path.Combine(file.DirectoryName, "src");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var newFileName = Path.Combine(folder, file.Name);
            //var newFileName = file.FullName;
            processedXml.Save(Processor.xmlWriter(newFileName));
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        private void showMessage(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string title = "Extension";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            throw new NotImplementedException();
        }

        public int OnBeforeSave(uint docCookie)
        {
            //showMessage("OnBeforeSave");
            //SaveProcessedFile(docCookie, "OnBeforeSave");
            return VSConstants.S_OK;

        }

        #endregion
    }
}
