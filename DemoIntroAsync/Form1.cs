using System.Diagnostics;

namespace DemoIntroAsync
{
    public partial class Form1 : Form
    {
        HttpClient httpClient = new HttpClient(); 

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencia = Path.Combine(directorioActual, @"Imagenes\resultado-secuencial");
            var destinoBaseParalelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecucion(destinoBaseParalelo, destinoBaseSecuencia);

            Console.WriteLine("Inicio");
            List<Imagen> imagenes = ObtenerImagenes();

            //Parte secuencial

            var sw = new Stopwatch();

            sw.Start();

            
            foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencia, imagen);
            }

            Console.WriteLine("Secuencial - duracion en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Reset();


            sw.Start();

            var tareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBaseParalelo, imagen);
            });

            await Task.WhenAll(tareasEnumerable);

            Console.WriteLine("paralelo - duracion en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Stop();

            pictureBox1.Visible=false;
        }

        private async Task ProcesarImagen (string directorio, Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;
            using(var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }

            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);
                    

        }

         private static  List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();

            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Bukele {i}.jpg",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Presidente_Nayib_Bukele_%28cropped%29.jpg/800px-Presidente_Nayib_Bukele_%28cropped%29.jpg"
                    });

                imagenes.Add(
                   new Imagen()
                   {
                       Nombre = $"Romero  {i}.jpg",
                       URL = "https://upload.wikimedia.org/wikipedia/commons/1/16/O.Romero_1979_autographed_photo.jpg"
                   });

                imagenes.Add(
                  new Imagen()
                  {
                      Nombre = $"tazumal  {i}.jpg",
                      URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/df/Templo_tazumal.jpg/800px-Templo_tazumal.jpg"
                  });

            }
            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecucion(string destinoBaseParalelo,
            string destinoBaseSecuencia)
        {
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            }

            if (!Directory.Exists(destinoBaseSecuencia))
            {
                Directory.CreateDirectory(destinoBaseSecuencia);
            }

            BorrarArchivos(destinoBaseSecuencia);
            BorrarArchivos(destinoBaseParalelo);

        }



        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(5000);
            return "Felipe";
        }

        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso A finalizado");
        }

        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso B finalizado");
        }

        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso C finalizado");
        }

    }
}
