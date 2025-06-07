using System;
using System.Diagnostics;
using System.Drawing;
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
        private const string AudioUrl = "https://cdn.discordapp.com/attachments/1260985603181379647/1380932673773764679/videoplayback-odh6xsak-1_g83yRxrw.mp3?ex=6845ad45&is=68445bc5&hm=942098c9f5c590b22ee273425e9b478ed49f51be253334fada1133c4766ab340&";
        private const string ImageUrl = "https://cdn.discordapp.com/attachments/1260985603181379647/1380153780020056064/image.png?ex=68457adf&is=6844295f&hm=7f03513ba4b73c89129c75d88d0447d2149e9a2e0c8603ff195c9b42f44ae783&";

        private const string AudioFileName = "joke_audio.mp3";
        private const string ImageFileName = "joke_image.png";

        private bool _allowClose = false;
        private string _userName = "";
        private uint originalVolume;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        private static extern bool BlockInput(bool fBlockIt);

        [DllImport("winmm.dll")]
        private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        [DllImport("winmm.dll")]
        private static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

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
                ShowFinalChoice();
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

        private void ShowFinalChoice()
        {
            Form choiceForm = new Form();
            choiceForm.Text = "System Alert";
            choiceForm.Size = new Size(400, 200);
            choiceForm.StartPosition = FormStartPosition.CenterScreen;
            choiceForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            choiceForm.MaximizeBox = false;
            choiceForm.MinimizeBox = false;
            choiceForm.TopMost = true;

            Label label = new Label();
            label.Text = "or we can do it the hard way";
            label.Location = new Point(20, 30);
            label.Size = new Size(350, 50);
            label.Font = new Font("Microsoft Sans Serif", 12);
            choiceForm.Controls.Add(label);

            Button easyButton = new Button();
            easyButton.Text = "Easy Way";
            easyButton.Location = new Point(80, 100);
            easyButton.Size = new Size(100, 35);
            easyButton.Click += async (s, e) => { choiceForm.Close(); await DoEasyWay(); };
            choiceForm.Controls.Add(easyButton);

            Button hardButton = new Button();
            hardButton.Text = "Hard Way";
            hardButton.Location = new Point(200, 100);
            hardButton.Size = new Size(100, 35);
            hardButton.Click += async (s, e) => { choiceForm.Close(); await DoHardWay(); };
            choiceForm.Controls.Add(hardButton);

            choiceForm.FormClosing += async (s, e) => await DoHardWay();
            choiceForm.ShowDialog();
        }

        private void SetVolumeToMax()
        {
            // Get current volume to restore later
            waveOutGetVolume(IntPtr.Zero, out originalVolume);

            // Set volume to maximum (0xFFFF for both left and right channels)
            uint maxVolume = 0xFFFF;
            waveOutSetVolume(IntPtr.Zero, ((uint)maxVolume & 0x0000ffff) | ((uint)maxVolume << 16));
        }

        private void RestoreOriginalVolume()
        {
            waveOutSetVolume(IntPtr.Zero, originalVolume);
        }

        private async Task DoEasyWay()
        {
            await PlayAudio();
            CleanupAndExit();
        }

        private async Task DoHardWay()
        {
            SetVolumeToMax();  // Set volume to max

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
                RestoreOriginalVolume();  // Restore volume

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
