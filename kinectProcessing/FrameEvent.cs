using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KINECTmania.kinectProcessing
{   
    /// <summary>
    /// Event um ein neues Bild anzukündigen und Bitmapadresse weiter zu geben
    /// </summary>
    public class BitmapGenerated : EventArgs
    {
        private WriteableBitmap wb;
        public WriteableBitmap Bitmap
        {
            get { return this.wb; }
            set { wb = value; }
        }

        public BitmapGenerated(WriteableBitmap wb)
        {
            this.wb = wb;
        }


    }
    /// <summary>
    /// Interface für BitmapGenerator
    /// </summary>
    public interface BitmapGenerator
    {
        event EventHandler<BitmapGenerated> RaiseBitmapGenerated;

        void OnRaiseBitmapGenerated(BitmapGenerated b);

    }
}
