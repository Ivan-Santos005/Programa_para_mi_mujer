using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CascadaRosa
{
    public partial class Form1 : Form
    {
        Timer timer;
        Random rand = new Random();

        // Configuración general
        string texto = "Te Amo";
        Color colorLetras = Color.FromArgb(255, 255, 105, 180); // Rosa brillante
        float velocidadCaida = 1.5f; // Más lento
        float velocidadExplosion = 0.3f; // Explosión más lenta
        int fontSize = 18;

        List<Letra> letras = new List<Letra>();
        List<Particula> particulas = new List<Particula>();
        Font fuente;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;

            fuente = new Font("Consolas", fontSize, FontStyle.Bold);

            // Crear letras iniciales
            for (int i = 0; i < 20; i++)
                letras.Add(new Letra(rand.Next(0, Width), rand.Next(-Height, 0), velocidadCaida));

            // Timer principal
            timer = new Timer();
            timer.Interval = 30; // Controla fluidez (más alto = más lento)
            timer.Tick += Timer_Tick;
            timer.Start();

            this.MouseDown += Form1_MouseDown;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Explode(e.X, e.Y);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var l in letras)
            {
                l.Y += l.Velocidad;
                if (l.Y > Height)
                {
                    l.Y = -rand.Next(100, 300);
                    l.X = rand.Next(0, Width);
                }
            }

            for (int i = particulas.Count - 1; i >= 0; i--)
            {
                particulas[i].Actualizar();
                if (!particulas[i].Viva)
                    particulas.RemoveAt(i);
            }

            Invalidate();
        }

        private void Explode(float x, float y)
        {
            int cantidad = 14 + rand.Next(12);
            for (int i = 0; i < cantidad; i++)
                particulas.Add(new Particula(x, y, texto, colorLetras, velocidadExplosion, fuente));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black); // 🔹 Limpia sin dejar rastros

            using (Brush b = new SolidBrush(colorLetras))
            {
                // Dibuja las letras de la cascada
                foreach (var l in letras)
                    g.DrawString(texto, fuente, b, l.X, l.Y);
            }

            // Dibuja partículas (explosiones)
            foreach (var p in particulas)
                p.Dibujar(g);
        }

        // ---- Clases internas ----
        public class Letra
        {
            public float X, Y, Velocidad;
            public Letra(float x, float y, float v)
            {
                X = x;
                Y = y;
                Velocidad = v;
            }
        }

        public class Particula
        {
            float x, y, vx, vy;
            int vida, edad;
            string texto;
            Brush brocha;
            Font fuente;
            bool viva = true;

            public bool Viva => viva;

            public Particula(float x, float y, string texto, Color color, float velocidad, Font fuente)
            {
                this.x = x;
                this.y = y;
                this.texto = texto;
                this.fuente = fuente;
                float ang = (float)(new Random().NextDouble() * Math.PI * 2);
                float mag = (float)(0.3 + new Random().NextDouble() * 1.5) * velocidad;
                vx = (float)Math.Cos(ang) * mag;
                vy = (float)Math.Sin(ang) * mag - 0.2f;
                vida = 180;
                edad = 0;
                brocha = new SolidBrush(color);
            }

            public void Actualizar()
            {
                vy += 0.02f;
                x += vx;
                y += vy;
                edad++;
                if (edad > vida)
                    viva = false;
            }

            public void Dibujar(Graphics g)
            {
                float alpha = 1f - (float)edad / vida;
                if (alpha < 0) alpha = 0;
                Color c = Color.FromArgb((int)(alpha * 255), ((SolidBrush)brocha).Color);
                using (Brush b = new SolidBrush(c))
                    g.DrawString(texto, fuente, b, x, y);
            }
        }
    }
}
