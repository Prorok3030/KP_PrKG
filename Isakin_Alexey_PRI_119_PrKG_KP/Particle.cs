using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace Isakin_Alexey_PRI_119_PrKG_KP
{
    internal class Particle
    {
        private float[] position = new float[3];
        private float _size;
        private float _lifeTime;

        private float[] Grav = new float[3];
        private float[] power = new float[3];
        private float attenuation;

        private float[] speed = new float[3];

        private float LastTime = 0;

        public Particle(float x, float y, float z, float size, float lifeTime, float start_time)
        {
            _size = size;
            _lifeTime = lifeTime;

            position[0] = x;
            position[1] = y;
            position[1] = z;

            speed[0] = 0;
            speed[1] = 0;
            speed[2] = 0;

            Grav[0] = 0;
            Grav[1] = -9.8f;
            Grav[2] = 0;

            attenuation = 3.33f;

            power[0] = 0;
            power[0] = 0;
            power[0] = 0;

            LastTime = start_time;
        }

        public void SetPower(float x, float y, float z)
        {
            power[0] = x;
            power[1] = y;
            power[2] = z;
        }

        public void InvertSpeed(int os, float attenuation)
        {
            speed[os] *= -1 * attenuation;
        }

        public float GetSize()
        {
            return _size;
        }

        public void setAttenuation(float new_value)
        {
            attenuation = new_value;
        }

        public void UpdatePosition(float timeNow)
        {
            float dTime = timeNow - LastTime;
            _lifeTime -= dTime;

            LastTime = timeNow;

            for (int a = 0; a < 3; a++)
            {
                if (power[a] > 0)
                {
                    power[a] -= attenuation * dTime;

                    if (power[a] <= 0)
                        power[a] = 0;
                }

                position[a] += (speed[a] * dTime + (Grav[a] + power[a]) * dTime * dTime);

                speed[a] += (Grav[a] + power[a]) * dTime;
            }
        }

        public bool isLife()
        {
            if (_lifeTime > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public float GetPositionX()
        {
            return position[0];
        }

        public float GetPositionY()
        {
            return position[1];
        }

        public float GetPositionZ()
        {
            return position[2];
        }
    }

    internal class Explosion
    {
        private float[] position = new float[3];
        private float _power;
        private int MAX_PARTICLES = 1000;
        private int _particles_now;

        private bool isStart = false;

        private Particle[] PartilceArray;

        private bool isDisplayList = false;
        private int DisplayListNom = 0;

        public Explosion(float x, float y, float z, float power, int particle_count)
        {
            position[0] = x;
            position[1] = y;
            position[2] = z;

            _particles_now = particle_count;
            _power = power;

            if (particle_count > MAX_PARTICLES)
            {
                particle_count = MAX_PARTICLES;
            }

            PartilceArray = new Particle[particle_count];
        }

        public void SetNewPosition(float x, float y, float z)
        {
            position[0] = x;
            position[1] = y;
            position[2] = z;
        }

        public void SetNewPower(float new_power)
        {
            _power = new_power;
        }

        private void CreateDisplayList()
        {
            DisplayListNom = Gl.glGenLists(1);

            Gl.glNewList(DisplayListNom, Gl.GL_COMPILE);

            Gl.glBegin(Gl.GL_TRIANGLES);

            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0.02f, 0.02f, 0);
            Gl.glVertex3d(0.02f, 0, -0.02f);

            Gl.glEnd();

            Gl.glEndList();

            isDisplayList = true;
        }

        public void Boooom(float time_start)
        {
            Random rnd = new Random();

            if (!isDisplayList)
            {
                CreateDisplayList();
            }

            for (int ax = 0; ax < _particles_now; ax++)
            {
                PartilceArray[ax] = new Particle(position[0], position[1], position[2], 50.0f, 1.5f, time_start);

                int direction_x = 1;
                int direction_y = 1;
                int direction_z = 1;


                float _power_rnd = rnd.Next((int)_power / 20, (int)_power);
                PartilceArray[ax].setAttenuation(_power / 2.0f);

                PartilceArray[ax].SetPower(_power_rnd * ((float)rnd.Next(100, 1000) / 1000.0f) * direction_x, _power_rnd * ((float)rnd.Next(100, 1000) / 1000.0f) * direction_y, _power_rnd * ((float)rnd.Next(100, 1000) / 1000.0f) * direction_z);
            }

            isStart = true;
        }

        public void Calculate(float time)
        {
            Random random = new Random();
            if (isStart)
            {
                for (int ax = 0; ax < _particles_now; ax++)
                {
                    if (PartilceArray[ax].isLife())
                    {
                        PartilceArray[ax].UpdatePosition(time);

                        Gl.glPushMatrix();
                        float size = PartilceArray[ax].GetSize();

                        Gl.glTranslated(PartilceArray[ax].GetPositionX(), PartilceArray[ax].GetPositionY(), PartilceArray[ax].GetPositionZ());
                        Gl.glScalef(size, size, size);
                        Gl.glRotated(random.Next(1, 180), 0, 0, 1);
                        Gl.glCallList(DisplayListNom);
                        Gl.glPopMatrix();

                        if (PartilceArray[ax].GetPositionY() < 0)
                        {
                            PartilceArray[ax].InvertSpeed(1, 0.6f);
                        }
                    }
                }
            }
        }
    }
}
