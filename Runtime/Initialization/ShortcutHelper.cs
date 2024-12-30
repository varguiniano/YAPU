using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Initialization
{
    /// <summary>
    /// Class to help create a shortcut of the application on windows.
    /// Credit to: https://stackoverflow.com/a/14632782/7575071
    /// </summary>
    public class ShortcutHelper
    {
        #if UNITY_STANDALONE_WIN

        /// <summary>
        /// Path to the borderless shortcut.
        /// </summary>
        public static string BorderlessShortcutPath =>
            Environment.CurrentDirectory + "\\" + Application.productName + "-Borderless.lnk ";

        /// <summary>
        /// Create a shortcut of the app that runs in borderless mode.
        /// </summary>
        public static void CreateBorderlessShortcut()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            IShellLink link = (IShellLink)new ShellLink();

            // setup shortcut information
            link.SetPath(Environment.CurrentDirectory + "\\" + Application.productName + ".exe");
            link.SetArguments("-popupwindow");

            // save it
            // ReSharper disable once SuspiciousTypeConversion.Global
            IPersistFile file = (IPersistFile)link;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            file.Save(BorderlessShortcutPath, false);
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink
        {
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        internal interface IShellLink
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
                         int cchMaxPath,
                         out IntPtr pfd,
                         int fFlags);

            void GetIDList(out IntPtr ppidl);

            void SetIDList(IntPtr pidl);

            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);

            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

            void GetHotkey(out short pwHotkey);

            void SetHotkey(short wHotkey);

            void GetShowCmd(out int piShowCmd);

            void SetShowCmd(int iShowCmd);

            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
                                 int cchIconPath,
                                 out int piIcon);

            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

            void Resolve(IntPtr hwnd, int fFlags);

            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }
        #endif
    }
}