using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Storage
{
    public static class TutorialStorage
    {
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tutorial_completed.txt");

        public static bool TutorialCompleted => System.IO.File.Exists(Path);

        public static async void SetTutorialCompleted(bool done)
        {
            if (done)
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    using (var sw = new System.IO.StreamWriter(Path))
                    {
                        sw.WriteLine("What is this file?");
                        sw.WriteLine("This file is to check if you've completed the tutorial.");
                        sw.Close();
                    }
                });
            }
            else
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    if (System.IO.File.Exists(Path)) System.IO.File.Delete(Path);
                });
            }
        }
    }
}
