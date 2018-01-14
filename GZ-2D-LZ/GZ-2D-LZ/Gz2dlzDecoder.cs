using System.Security.Permissions;
using System.Xml;

namespace G2_2D_LZ
{
    public class Gz2dlzDecoder
    {
        private string _inputFileName;

        public Gz2dlzDecoder(string inputFileName)
        {
            _inputFileName = inputFileName;
        }

        public void Decode()
        {
            //decode the matching tables
            //predic one row of pixels and correct each prediction error using the prediction error table
            //advance the decoder to the next undecoded pixel
            //while ( more pixeld to be decoded)
                //read a value from the match flag table
                //if (match flag is true)
                    //reproduce a block o pixels and correct the prediction errors using the prediction error table
                //if (match falg is false)
                    //predict a block of pixells and correct the prediction errors using the prediction error table
                //get the next unencoded pixel
            //end while
            //output the reconstructed image
        }
    }
}
