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
using System.IO;
using System.Diagnostics;
namespace Graphics

{
    class Renderer
    {
        Shader sh;

        uint cheeseBufferID;
        uint armsBufferID;
        uint legsBufferID;
        uint mouthBufferID;
        uint eyesBufferID;
   
        mat4 ModelMatrix;
        mat4 ViewMatrix;
        mat4 ProjectionMatrix;

        int ShaderModelMatrixID;
        int ShaderViewMatrixID;
        int ShaderProjectionMatrixID;

        const float rotationSpeed = 1f;
        float rotationAngle = 0;

        public float translationX = 0,
                     translationY = 0,
                     translationZ = 0;

        Stopwatch timer = Stopwatch.StartNew();

        vec3 triangleCenter;

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            Gl.glClearColor(0, 0, 0.4f, 1);

            triangleCenter = new vec3(-1.0f, -1.0f, -0.2f);
            float[] cheeseVerts = {
            -1.0f,  -1.0f,  0.2f,
            1.0f, 0.6f, 0.3f,//RGB
            1.0f,  -1.0f,  0.2f,
            1.0f, 0.6f, 0.3f,//RGB
            0.0f, 1.0f,  0.2f,
            1.0f, 0.6f, 0.9f//RGB 
            };
            float[] eyesVerts = {
            -0.5f,-0.7f,0.2f,
            0.0f,0.0f,0.0f,//RGB
            0.5f,-0.7f,0.2f,
            0.0f,0.0f,0.0f,//RGB
            };
            float[] mouthVerts = {        
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
            };
          
            float[] armsVerts = {
                
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
            };
            float[] legsVerts = {
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


            cheeseBufferID =  GPU.GenerateBuffer(cheeseVerts); 
            armsBufferID =  GPU.GenerateBuffer(armsVerts); 
            legsBufferID =  GPU.GenerateBuffer(legsVerts); 
            mouthBufferID =  GPU.GenerateBuffer(mouthVerts); 
            eyesBufferID = GPU.GenerateBuffer(eyesVerts);


            // View matrix 
            ViewMatrix = glm.lookAt(
            new vec3(0, 0, 2),// eye
            new vec3(0, 0, 0), // center
            new vec3(0, 1, 0)); // up

            // Model Matrix Initialization
            ModelMatrix = new mat4(1);

            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            ProjectionMatrix = glm.perspective(-45.0f, 4.0f / 3.0f, 0.1f, 100.0f);

            // Our MVP matrix which is a multiplication of our 3 matrices 
            sh.UseShader();


            //Get a handle for our "MVP" uniform (the holder we created in the vertex shader)
            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");

            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

            timer.Start();
        }

        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
           

            #region Cheese
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, cheeseBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 3);


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion
           
            #region Eyes
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, eyesBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glPointSize(10);
            Gl.glDrawArrays(Gl.GL_POINTS, 0, 2);


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion
            #region Arms
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, armsBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glLineWidth(5);
            Gl.glDrawArrays(Gl.GL_LINES, 0, 4);


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion
            #region Legs
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, legsBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glLineWidth(5);
            Gl.glDrawArrays(Gl.GL_LINE_STRIP, 0, 4);



            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion
            #region Mouth
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, mouthBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);



            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion
         
        }

        public void Update()
        {
            timer.Stop();
            var deltaTime = timer.ElapsedMilliseconds / 1000.0f;
            rotationAngle += deltaTime * rotationSpeed;

            List<mat4> transformations = new List<mat4>();
            transformations.Add(glm.translate(new mat4(1), -1 * triangleCenter));
            transformations.Add(glm.rotate(rotationAngle, new vec3(1, 0, 0)));
            transformations.Add(glm.translate(new mat4(1), triangleCenter));
            transformations.Add(glm.translate(new mat4(1), new vec3(translationX, translationY, translationZ)));

            ModelMatrix = MathHelper.MultiplyMatrices(transformations);

            timer.Reset();
            timer.Start();
        }

        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
