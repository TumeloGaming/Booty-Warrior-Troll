using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace JokeProgram
{
    public partial class MainForm : Form
    {
        private const string AudioUrl = "https://cdn.discordapp.com/attachments/1260985603181379647/1380155235003142154/videoplayback_OdH6xsaK.mp3?ex=6842d93a&is=684187ba&hm=e64a550df06f0db17d24463565275514b33986da626ceb886b491deb2b5c363d&";
        private const string ImageUrl = "https://cdn.discordapp.com/attachments/1260985603181379647/1380153780020056064/image.png?ex=6842d7df&is=6841865f&hm=af582807c4ab4961458025237952722cda774a4c4dd7a1c56abf855a241f6862&";

        private const string AudioFileName = "joke_audio.mp3";
        private const string ImageFileName = "joke_image.png";

        private bool _allowClose = false;
        private string _userName = "";

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        private static extern bool BlockInput(bool fBlockIt);

        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            this.Hide();
            await StartJokeSequence();
        }

        private async Task StartJokeSequence()
        {
            try
            {
                // Step 1: Ask for name
                _userName = Interaction.InputBox("Please enter your name:", "System Error", "");

                if (string.IsNullOrEmpty(_userName))
                {
                    _userName = "User";
                }

                // Step 2: "I came looking for booty"
                MessageBox.Show("I came looking for booty", "System Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Step 3: "I came looking for mans butt"
                MessageBox.Show("I came looking for mans butt", "System Alert", MessageBoxButtons.OK, MessageBoxIcon.Question);

                // Step 4: "oh i know who you are (name)-handsome"
                MessageBox.Show($"oh i know who you are {_userName}-handsome", "System Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Step 5: "Now i tell you what..."
                MessageBox.Show("Now i tell you what...", "System Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Step 6: "I likes you"
                MessageBox.Show("I likes you", "System Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Step 7: "And i want you"
                MessageBox.Show("And i want you", "System Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Step 8: "Now we can do this the easy way..."
                MessageBox.Show("Now we can do this the easy way...", "System Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Step 9: Download files
                await DownloadFiles();

                // Step 10: Final choice
                var result = MessageBox.Show("or we can do it the hard way", "System Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await DoEasyWay();
                }
                else
                {
                    await DoHardWay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanupAndExit();
            }
        }

        private async Task DownloadFiles()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Download audio file
                    var audioData = await client.GetByteArrayAsync(AudioUrl);
                    await File.WriteAllBytesAsync(AudioFileName, audioData);

                    // Download image file
                    var imageData = await client.GetByteArrayAsync(ImageUrl);
                    await File.WriteAllBytesAsync(ImageFileName, imageData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to download files: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task DoEasyWay()
        {
            await PlayAudio();
            CleanupAndExit();
        }

        private async Task DoHardWay()
        {
            // Change wallpaper
            if (File.Exists(ImageFileName))
            {
                string fullImagePath = Path.GetFullPath(ImageFileName);
                SystemParametersInfo(20, 0, fullImagePath, 3);
            }

            // Block input and play audio 10 times
            try
            {
                BlockInput(true);

                for (int i = 0; i < 10; i++)
                {
                    await PlayAudio();
                    await Task.Delay(1000);
                }
            }
            finally
            {
                BlockInput(false);
            }

            CleanupAndExit();
        }

        private async Task PlayAudio()
        {
            try
            {
                if (File.Exists(AudioFileName))
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-Command \"Add-Type -AssemblyName presentationCore; $player = New-Object system.windows.media.mediaplayer; $player.open('{Path.GetFullPath(AudioFileName)}'); $player.Play(); Start-Sleep -Seconds 5; $player.Stop()\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    };

                    var process = Process.Start(psi);
                    if (process != null)
                    {
                        await Task.Run(() => process.WaitForExit());
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(AudioFileName) { UseShellExecute = true });
                    await Task.Delay(5000);
                }
                catch
                {
                    MessageBox.Show($"Could not play audio: {ex.Message}", "Audio Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void CleanupAndExit()
        {
            try
            {
                if (File.Exists(AudioFileName))
                    File.Delete(AudioFileName);

                if (File.Exists(ImageFileName))
                    File.Delete(ImageFileName);
            }
            catch { }

            _allowClose = true;
            Application.Exit();
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowClose)
            {
                e.Cancel = true;
                MessageBox.Show($"Nice try, {_userName}! You can't escape that easily!\nNow you're going the Hard Way!", "No Escape!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                await DoHardWay();
            }
        }
    }
}
