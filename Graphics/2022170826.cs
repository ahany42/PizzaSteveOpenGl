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
         // Cheese
          -1.0f,  -1.0f,  0.2f,
          1.0f, 0.8f, 0.3f,//RGB
          1.0f,  -1.0f,  0.2f,  
          1.0f, 0.8f, 0.3f,//RGB
          0.0f, 1.0f,  0.2f,  
          1.0f, 0.8f, 0.3f,//RGB

         // Pizza 
         -1.0f,  -1.0f, -0.2f, 
         1.0f, 0.6f, 0.3f,//RGB
         1.0f,  -1.0f, -0.2f,
         1.0f, 0.6f, 0.3f,//RGB
         0.0f, 1.0f, -0.2f,
        1.0f, 0.6f, 0.3f,//RGB  

         // Eyes
         -0.5f,-0.7f,0.2f,
         0.0f,0.0f,0.0f,//RGB
         0.5f,-0.7f,0.2f,
         0.0f,0.0f,0.0f,//RGB

        // Mouth Quad - 2 Triangles
        -0.2f, 0.2f, 0.2f,   
        0.0f, 0.0f, 0.0f,//RGB  
        0.2f, 0.2f, 0.2f,   
        0.0f, 0.0f, 0.0f,//RGB  
        0.2f, 0.4f, 0.2f,    
        0.0f, 0.0f, 0.0f,//RGB  
        -0.2f, 0.2f, 0.2f,
        0.0f, 0.0f, 0.0f,//RGB  
        0.2f, 0.4f, 0.2f,  
        0.0f, 0.0f, 0.0f,//RGB 
        -0.2f, 0.4f, 0.2f,
        0.0f, 0.0f, 0.0f,//RGB

        //Right Arm
        -0.5f,0.0f,0.2f,
        0.0f,0.0f,0.0f,//RGB
        -1.4f,-1.0f,0.2f,
        0.0f,0.0f,0.0f,//RGB

        //Left Arm
        0.5f,0.0f,0.2f,
        0.0f,0.0f,0.0f,//RGB
        1.4f,-1.0f,0.2f,
        0.0f,0.0f,0.0f,//RGB

        // Legs (Using Line Strip)
        -0.4f, 1.5f, 0.2f,   
        0.0f, 0.0f, 0.0f,//RGB
        -0.4f, 1.0f, 0.2f,   
        0.0f, 0.0f, 0.0f,//RGB
         0.4f, 1.0f, 0.2f,   
        0.0f, 0.0f, 0.0f,//RGB
        0.4f, 1.5f, 0.2f,   
        0.0f, 0.0f, 0.0f//RGB


   };           
            vertexBufferID = GPU.GenerateBuffer(verts);

            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            p = glm.perspective(5.0f,1/1.0f,1.0f, 10.0f);
            // View matrix 
            v = glm.lookAt(
                new vec3(0,0,2),// eye
                new vec3(0,0,0), // center
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
             // 5 Primitives Triangles,Points,Lines,Line Strip,Square(using 2 triangles)
            //pizza slice
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //eyes
            Gl.glPointSize(10);
            Gl.glDrawArrays(Gl.GL_POINTS, 6, 2);

            //Mouth Quad
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 8, 6);

            //Arms
            Gl.glLineWidth(5);
            Gl.glDrawArrays(Gl.GL_LINES, 14, 4);

            // Draw Legs using Line Strip
            Gl.glLineWidth(5);
            Gl.glDrawArrays(Gl.GL_LINE_STRIP, 18, 4);  

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
