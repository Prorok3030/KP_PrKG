using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.DevIl;

namespace Isakin_Alexey_PRI_119_PrKG_KP
{
    public partial class Form1 : Form
    {
        int angle = 3, angleX = -90, angleY = 0, angleZ = 90;
        int translateX = -80, translateY = -40, translateZ = -50;
        int alpha = 0;
        float darkIndex = 0;

        private int imageId;
        private uint pictureTexture;
        string picture = "picture.jpg";

        float beta = 0;
        double spiderCoordZ = 0;
        double lampCoordZ = 0;
        Boolean drop = false;
        Boolean lampState = true;

        private float global_time = 0; private float[,] camera_date = new float[5, 7];
        private Explosion BOOOOM_1 = new Explosion(1, 10, 1, 100, 100);



        private void Form1_Load(object sender, EventArgs e)
        {

            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);
            Gl.glClearColor(255, 255, 255, 1);
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(60, (float)AnT.Width / (float)AnT.Height, 0.1, 900);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            Il.ilGenImages(1, out imageId);
            Il.ilBindImage(imageId);

            if (Il.ilLoadImage(picture))
            {
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp)
                {
                    case 24:
                        pictureTexture = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        pictureTexture = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
            }
            Il.ilDeleteImages(1, ref imageId);
            comboBox1.SelectedIndex = 0;


            RenderTimer.Start();
        }

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)
        {

            uint texObject;
            Gl.glGenTextures(1, out texObject);
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);

            switch (Format)
            {

                case Gl.GL_RGB:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

                case Gl.GL_RGBA:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

            }
            return texObject;
        }
            private void RenderTimer_Tick(object sender, EventArgs e)
        {
            global_time += (float)RenderTimer.Interval / 1000;
            Draw();
        }

        private void помощьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form help = new Help();
            help.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                angle = 3; angleX = -90; angleY = 0; angleZ = 90;
                translateX = -80; translateY = -40; translateZ = -50;
            }

            if (comboBox1.SelectedIndex == 1)
            {
                angle = 3; angleX = -90; angleY = 0; angleZ = 90;
                translateX = -170; translateY = -30; translateZ = -60;
            }

            if (comboBox1.SelectedIndex == 2)
            {
                angle = 3; angleX = -90; angleY = 0; angleZ = 90;
                translateX = -170; translateY = -70; translateZ = -70;
            }

            if (comboBox1.SelectedIndex == 3)
            {
                angle = 3; angleX = -140; angleY = 0; angleZ = 90;
                translateX = -180; translateY = 0; translateZ = -55;
            }
            if (comboBox1.SelectedIndex == 4)
            {
                angle = 3; angleX = -90; angleY = 0; angleZ = 90;
                translateX = -140; translateY = -40; translateZ = -90;
            }
        }


        private void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glClearColor(255, 255, 255, 1);
            Gl.glLoadIdentity();
            Gl.glPushMatrix();
            Gl.glRotated(angleX, 1, 0, 0); Gl.glRotated(angleY, 0, 1, 0); Gl.glRotated(angleZ, 0, 0, 1);
            Gl.glTranslated(translateX, translateY, translateZ);
            //Gl.glColor3f(0.07f, 0.04f, 0.56f);
            BOOOOM_1.Calculate(global_time);

            //Стены и потолок
            Gl.glPushMatrix();
            Gl.glColor3f(0f + darkIndex, 0.8f + darkIndex, 0.8f + darkIndex);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(200, 200, 0);
            Gl.glVertex3d(200, -10, 0);
            Gl.glVertex3d(200, -10, 100);
            Gl.glVertex3d(200, 200, 100);
            Gl.glEnd();

            Gl.glPushMatrix();
            Gl.glColor3f(0f + darkIndex, 0.8f + darkIndex, 0.8f + darkIndex);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(-100, -10, 0);
            Gl.glVertex3d(200, -10, 0);
            Gl.glVertex3d(200, -10, 100);
            Gl.glVertex3d(-100, -10, 100);
            Gl.glEnd();

            Gl.glPushMatrix();
            Gl.glColor3f(0f + darkIndex, 0f + darkIndex, 0f + darkIndex);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(-100, -10, 0);
            Gl.glVertex3d(200, -10, 0);
            Gl.glVertex3d(200, -10, 100);
            Gl.glVertex3d(-100, -10, 100);
            Gl.glEnd();

            Gl.glColor3f(0.8f + darkIndex, 0.8f + darkIndex, 0.8f + darkIndex);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex3d(200, 200, 100);
            Gl.glVertex3d(-100, 200, 100);
            Gl.glVertex3d(-100, -10, 100);
            Gl.glVertex3d(200, -10, 100);
            Gl.glEnd();

            //Пол
            Gl.glColor3f(0.5f + darkIndex, 0.5f + darkIndex, 0f + darkIndex);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex3d(200, 200, 0);
            Gl.glVertex3d(-100, 200, 0);
            Gl.glVertex3d(-100, -10, 0);
            Gl.glVertex3d(200, -10, 0);
            Gl.glEnd();

            //Часы
            Gl.glPushMatrix();
            Gl.glTranslated(200, 30, 0);
            Gl.glColor3f(0.2f + darkIndex, 0.3f + darkIndex, 0f + darkIndex);
            Gl.glScaled(1, 1.2, 7);
            Glut.glutSolidCube(15);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1f);
            Glut.glutWireCube(15);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(200, 30, 60);
            Gl.glColor3f(0.3f + darkIndex, 0.4f + darkIndex, 0f + darkIndex);
            Gl.glScaled(1, 1.2, 1);
            Glut.glutSolidCube(15);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Glut.glutWireCube(15);
            Gl.glPopMatrix();

            //Циферблат
            Gl.glPushMatrix();
            Gl.glTranslated(202, 35, 55);
            Gl.glColor3f(1f + darkIndex, 1f + darkIndex, 0.8f + darkIndex);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(-10, 0, 0);
            Gl.glVertex3d(-10, -10, 0);
            Gl.glVertex3d(-10, -10, 10);
            Gl.glVertex3d(-10, 0, 10);
            Gl.glEnd();
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glPopMatrix();

            //Стрелки
            Gl.glPushMatrix();
            Gl.glTranslated(192, 30, 60);
            Gl.glColor3f(0,0,0);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glLineWidth(1f);
            Gl.glRotatef(alpha, 1, 0, 0);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0, 0, 3.5);
            Gl.glEnd();
            alpha += 12;
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(192, 30, 60);
            Gl.glColor3f(0, 0, 0);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glLineWidth(2f);
            Gl.glRotatef(beta, 1, 0, 0);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0, 0, 2);
            Gl.glEnd();
            beta += 1;
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glPopMatrix();

            //Люстра
            Gl.glPushMatrix();
            Gl.glTranslated(170, 40, 100);
            Gl.glColor3f(0.47f + darkIndex, 0.52f + darkIndex, 0.48f + darkIndex);
            Gl.glScaled(0.3, 0.3, 5);
            Glut.glutSolidCube(5);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1f);
            Glut.glutWireCube(5);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(170, 40, 100);
            Gl.glColor3f(0.47f + darkIndex, 0.52f + darkIndex, 0.48f + darkIndex);
            Gl.glScaled(0.3, 1, 0.3);
            Glut.glutSolidCube(5);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1f);
            Glut.glutWireCube(5);
            Gl.glPopMatrix();

            if(drop == true && lampState == true)
            {
                darkIndex = -0.5f;
                Gl.glPushMatrix();
                Gl.glTranslated(170, 40, 85 + lampCoordZ);
                Gl.glColor3f(0.8f, 0.8f, 0.6f);
                Glut.glutSolidSphere(5, 20, 20);
                Gl.glColor3f(0, 0, 0);
                Gl.glLineWidth(1f);
                Glut.glutWireSphere(6, 8, 8);
                Gl.glPopMatrix();
                lampCoordZ -= 20;

                if(lampCoordZ <= -100)
                {
                    BOOOOM_1.SetNewPosition(170, 40, 40);
                    BOOOOM_1.SetNewPower(500);
                    BOOOOM_1.Boooom(global_time);
                    lampCoordZ = 0;
                    lampState = false;
                }
            }
            else
            {
                if (lampState == true)
                {
                    Gl.glPushMatrix();
                    Gl.glTranslated(170, 40, 85);
                    Gl.glColor3f(0.9f, 0.9f, 0.3f);
                    Glut.glutSolidSphere(5, 20, 20);
                    Gl.glColor3f(0, 0, 0);
                    Gl.glLineWidth(1f);
                    Glut.glutWireSphere(6, 8, 8);
                    Gl.glPopMatrix();
                }
            }
        
            //Паутина
            Gl.glPushMatrix();
            Gl.glTranslated(200, -10, 100);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1f);
            Glut.glutWireSphere(8, 20, 20);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(198, -8, 100);
            Gl.glColor3f(0f, 0f, 0f);
            Gl.glLineWidth(1f);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0, 0, -35 + spiderCoordZ);
            Gl.glEnd();
            Gl.glPopMatrix();

            //Паук
            Gl.glPushMatrix();
            Gl.glTranslated(198, -8, 65 + spiderCoordZ);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1f);
            Glut.glutSolidSphere(1, 20, 20);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(198, -8, 64 + spiderCoordZ);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1f);
            Glut.glutSolidSphere(0.5, 20, 20);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(198, -6, 65 + spiderCoordZ);
            Gl.glColor3f(0f, 0f, 0f);
            Gl.glLineWidth(2f);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0, -4, 0);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(198, -8, 65 + spiderCoordZ);
            Gl.glRotated(45, 1, 0 ,0);
            Gl.glColor3f(0f, 0f, 0f);
            Gl.glLineWidth(2f);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3d(0, 2, 0);
            Gl.glVertex3d(0, -2, 0);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(198, -8, 65 + spiderCoordZ);
            Gl.glRotated(-45, 1, 0, 0);
            Gl.glColor3f(0f, 0f, 0f);
            Gl.glLineWidth(2f);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3d(0, 2, 0);
            Gl.glVertex3d(0, -2, 0);
            Gl.glEnd();
            Gl.glPopMatrix();

            //Комод
            Gl.glPushMatrix();
            Gl.glTranslated(200, 0, 10);
            Gl.glColor3f(0.2f + darkIndex, 0.3f + darkIndex, 0f + darkIndex);
            Gl.glScaled(3, 1, 1);
            Glut.glutSolidCube(25);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(3f);
            Glut.glutWireCube(25);
            Gl.glScaled(1, 1, 2);
            Glut.glutWireCube(25);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(180, 2, 20);
            Gl.glColor3f(0, 0, 0);
            Gl.glScaled(1, 0.95, 0.05);
            Glut.glutSolidCube(25);
            Gl.glColor3f(0, 0, 0);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(200, 0, 22.6);
            Gl.glColor3f(0.2f, 0.2f, 0.2f);
            Gl.glScaled(3, 1, 0.01);
            Glut.glutSolidCube(25);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(2f);
            Glut.glutWireCube(25);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(200, 0, 35);
            Gl.glColor3f(0.2f, 0.2f, 0.2f);
            Gl.glScaled(3, 1, 0.01);
            Glut.glutSolidCube(25);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1f);
            Glut.glutWireCube(25);
            Gl.glPopMatrix();

            //Фрактал
            Gl.glPushMatrix();
            Gl.glTranslated(200, 50, 55);
            Gl.glScaled(0.5, 0.3, 0.6);
            Gl.glRotated(90, 0, 1, 0);
            Gl.glRotated(90, 0, 0, 1);
            Gl.glColor3f(0f, 0f, 0f);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glLineWidth(3f);
            Gl.glBegin(Gl.GL_LINES);
            dragonCurve(0, 0, 100, 0, 0);
            Gl.glEnd();
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glPopMatrix();

            //Картина
            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, pictureTexture);
            Gl.glPushMatrix();
            Gl.glTranslated(200, 75, 80);
            Gl.glRotated(90, 1, 0, 0);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(0, 0, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(0, -10, 0);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(0, -10, 10);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(0, 0, 10);
            Gl.glTexCoord2f(0, 1);
            Gl.glEnd();
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(199.5, 75, 70);
            Gl.glColor3f(0.5f + darkIndex, 0.6f + darkIndex, 0.2f + darkIndex);
            Gl.glLineWidth(8f);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0, -10, 0);
            Gl.glVertex3d(0, -10, 10);
            Gl.glVertex3d(0, 0, 10);
            Gl.glEnd();
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glPopMatrix();



            Gl.glPopMatrix();
            Gl.glFlush();
            AnT.Invalidate();

        }

        private void AnT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                 translateY -= 1;
            }
            if (e.KeyCode == Keys.D)
            {
                translateY += 1;
            }
            if (e.KeyCode == Keys.W)
            {
                translateX -= 1;
            }
            if (e.KeyCode == Keys.S)
            {
                translateX += 1;
            }

            if (e.KeyCode == Keys.Z)
            {
                angleZ -= angle;
            }
            if (e.KeyCode == Keys.X)
            {
                angleZ += angle;
            }

            if (e.KeyCode == Keys.NumPad8 && spiderCoordZ <= 25)
            {
                spiderCoordZ += 1;
            }
            if (e.KeyCode == Keys.NumPad2 && spiderCoordZ >= 0)
            {
                spiderCoordZ -= 1;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            drop = true;
            AnT.Focus();
            button1.Enabled = false;
        }

        void dragonCurve(int x1, int y1, int x2, int y2, int i)
        {
            if (i == 15)
            {
                Gl.glColor3f(0, 0, 0);
                Gl.glVertex2i(x1, y1);
                Gl.glVertex2i(x2, y2);
            }
            else
            {
                int x = (x1 + x2) / 2 + (y2 - y1) / 2;
                int y = (y1 + y2) / 2 - (x2 - x1) / 2; 
                dragonCurve(x, y, x1, y1, i + 1);
                dragonCurve(x, y, x2, y2, i + 1);
            }
        }
    }
}
