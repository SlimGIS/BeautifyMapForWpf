using ICSharpCode.AvalonEdit.Highlighting;
using SlimGis.MapKit.Geometries;
using SlimGis.MapKit.Symbologies;
using SlimGis.MapKit.Wpf;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeautifyMap_Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            Map1.MapUnit = GeoUnit.Meter;

            LayerOverlay overlay = new LayerOverlay();
            Map1.Overlays.Add(overlay);

            LoadFromTheme(overlay, "Dark");
            Map1.ZoomTo(overlay.Layers["texas_knownstreets"].GetBound());
        }

        private void LoadFromTheme(LayerOverlay overlay, string themeName)
        {
            string fileName = $"{themeName}Theme.sgcss";

            textEditor.Load(fileName);
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.HighlightingDefinitions.Where(d => d.Name.Equals("CSS")).First();

            string css = File.ReadAllText(fileName);
            LoadFromCss(overlay, css);
        }

        private void LoadFromCss(LayerOverlay overlay, string css)
        {
            CssDocument doc = CssDocument.Parse(css);
            overlay.LoadStyledLayers("AppData", doc);

            GeoSolidBrush background = doc.Styles.First().FillBrush as GeoSolidBrush;
            if (background != null)
            {
                Map1.Background = new SolidColorBrush(Color.FromArgb(background.Color.Alpha, background.Color.Red, background.Color.Green, background.Color.Blue));
            }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            string themeName = ((ComboBoxItem)combobox.SelectedValue).Content.ToString();

            LayerOverlay overlay = Map1.Overlays.OfType<LayerOverlay>().FirstOrDefault();
            if (overlay == null) return;

            LoadFromTheme(overlay, themeName);
            Map1.Overlays[0].Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LayerOverlay overlay = Map1.Overlays.OfType<LayerOverlay>().FirstOrDefault();
            if (overlay == null) return;

            LoadFromCss(overlay, textEditor.Text);
            Map1.Overlays[0].Refresh();
        }
    }
}
