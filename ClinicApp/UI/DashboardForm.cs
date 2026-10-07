PictureBox picLogo = new PictureBox { Size = new Size(60, 60), SizeMode = PictureBoxSizeMode.Zoom };
string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");

try
{
    if (File.Exists(logoPath))
    {
        using (var fs = new FileStream(logoPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            picLogo.Image = Image.FromStream(fs);
        }
    }
    else
    {
        // Fallback: Create a simple colored box with "V" if logo.png is missing
        picLogo.BackColor = Color.DodgerBlue;
        picLogo.SizeMode = PictureBoxSizeMode.CenterImage;
        // (Optional) You can remove the next line if you just want a blue box
        // picLogo.Image = CreateTextImage("V", 60, 60); 
    }
}
catch
{
    picLogo.BackColor = Color.LightGray; // Safe fallback on any image error
}
