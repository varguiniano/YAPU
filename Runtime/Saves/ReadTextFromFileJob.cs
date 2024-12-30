using System.IO;
using System.Text;
using Unity.Collections;
using Unity.Jobs;

namespace Varguiniano.YAPU.Runtime.Saves
{
    /// <summary>
    /// Job that reads the lines in a file.
    /// </summary>
    public struct ReadTextFromFileJob : IJob
    {
        /// <summary>
        /// File to read.
        /// </summary>
        public NativeArray<byte> Path;

        /// <summary>
        /// Data read.
        /// </summary>
        public NativeQueue<byte> Data;

        /// <summary>
        /// Read the data from the file.
        /// </summary>
        public void Execute()
        {
            foreach (byte data in Encoding.UTF8.GetBytes(File.ReadAllText(Encoding.UTF8.GetString(Path))))
                Data.Enqueue(data);
        }
    }
}