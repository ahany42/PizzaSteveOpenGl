/*
  Aly Hany Mohamed
  2022170826
  AI
  Section 2
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
//include GLM library


using System.IO;

namespace Graphics
{
    class Renderer
    {
        Shader sh;
        uint vertexBufferID;

        //3D Drawing
        mat4 m;
        mat4 v;
        mat4 p;
        mat4 mvp;
        int MVP_ID;
        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            Gl.glClearColor(0, 0, 0.4f, 1);
            float[] verts = { 
		        // T1
		        0.5f, 0.0f, -3.0f,
                1.0f, 0.0f, 0.0f, //R

	            0.0f, 1.0f, -3.0f,
                0.0f, 1.0f, 0.0f, //G

		        0.0f, -1.0f, -3.0f,
                0.0f, 0.0f, 1.0f,  //B
		
		        //T2
		        -0.5f, 0.0f, -6.0f,
                1.0f, 1.0f, 0.0f, //
	            0.0f, 1.0f, -6.0f,
                1.0f, 0.0f, 1.0f, //
		        0.0f, -1.0f, -6.0f,
                0.0f, 1.0f, 1.0f,  //

		        //Axis
		        //x
		        0.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f, //R
		        5.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f, //R
		        //y
	            0.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f, //G
		        0.0f, 5.0f, 0.0f,
                0.0f, 1.0f, 0.0f, //G
		        //z
	            0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f,  //B
		        0.0f, 0.0f, -5.0f,
                0.0f, 0.0f, 1.0f,  //B
            };


            vertexBufferID = GPU.GenerateBuffer(verts);

            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            p = glm.perspective(45, 4 / 3.0f, 0.1f, 100);
            // View matrix 
            v = glm.lookAt(
                new vec3(3,5,5),// eye
                new vec3(0,2,0), // center
                new vec3(0,1,0)); // up
            // Model matrix: apply transformations to the model
            m = new mat4(1);
            // Our MVP matrix which is a multiplication of our 3 matrices 
            List<mat4> mvpList = new List<mat4>();
            mvpList.Add(m);
            mvpList.Add(v);
            mvpList.Add(p);
            mvp = MathHelper.MultiplyMatrices(mvpList);

            sh.UseShader();


            //Get a handle for our "MVP" uniform (the holder we created in the vertex shader)
            MVP_ID = Gl.glGetUniformLocation(sh.ID, "MVP");
            //pass the value of the MVP you just filled to the vertex shader
            Gl.glUniformMatrix4fv(MVP_ID, 1, Gl.GL_FALSE, mvp.to_array());
        }

        public void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6*sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3*sizeof(float)));

            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
            Gl.glDrawArrays(Gl.GL_LINES, 6, 6);



            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
        }
        public void Update()
        {
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
