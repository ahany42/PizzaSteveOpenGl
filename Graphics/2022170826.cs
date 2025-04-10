﻿/*
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
using static System.Net.Mime.MediaTypeNames;
namespace Graphics

{
    class Renderer
    {
        Shader sh;

        uint cheeseBufferID;
        uint cheese2BufferID;
        uint armsBufferID;
        uint legsBufferID;
        uint mouthBufferID;
        uint eyesBufferID;

        mat4 ModelMatrix;
        mat4 ViewMatrix;
        mat4 ProjectionMatrix;

        Texture tex1;

        int ShaderModelMatrixID;
        int ShaderViewMatrixID;
        int ShaderProjectionMatrixID;
        int useTextureID;
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
            tex1 = new Texture(projectPath + "\\Textures\\pizza.png", 1);
            Gl.glClearColor(1f, 1f, 1f, 1);

            triangleCenter = new vec3(-1.0f, -1.0f, -0.2f);
            //5 primitives(triangles, line, line strip, points, 2 triangles forming quad(mouth))
            //3D Pyramid Cheese Using Traingles
            float[] cheeseVerts = {
                  -1.0f,  -1.0f,  0.2f,
            1.0f, 0.6f, 0.3f,
            0,0,
            1.0f,  -1.0f,  0.2f,
            1.0f, 0.6f, 0.3f,
            0,1,
            0.0f,   1.0f,  0.2f,
            1.0f, 0.6f, 0.9f,
            1,0,
            0.0f, -0.33f, 1.0f,
            1.0f, 1.0f, 0.3f,
            0,0,
            -1.0f,  -1.0f,  0.2f,
            1.0f, 0.6f, 0.3f,
            1,0,
            1.0f,  -1.0f,  0.2f,
            1.0f, 0.6f, 0.3f,
            0,1,
            0.0f, -0.33f, 1.0f,
            1.0f, 1.0f, 0.3f,
            1,1,
            1.0f,  -1.0f,  0.2f,
            1.0f, 0.6f, 0.3f,
            0,0,
            0.0f,   1.0f,  0.2f,
            1.0f, 0.6f, 0.9f,
            1,0,
            0.0f, -0.33f, 1.0f,
            1.0f, 1.0f, 0.3f,
           0,1,
            0.0f,   1.0f,  0.2f,
            1.0f, 0.6f, 0.9f,
            1,0,
            -1.0f,  -1.0f,  0.2f,
            1.0f, 0.6f, 0.3f,
            1,1,
            };
            float[] eyesVerts = {
            -0.5f,-0.7f,0.2f,
            0.0f,0.0f,0.0f,
            0.5f,-0.7f,0.2f,
            0.0f,0.0f,0.0f,
            };
            float[] mouthVerts = {        
             // Mouth Quad - 2 Triangles
             -0.2f, 0.2f, 0.2f,
             0.0f, 0.0f, 0.0f,
             0.2f, 0.2f, 0.2f,
             0.0f, 0.0f, 0.0f,
             0.2f, 0.4f, 0.2f,
             0.0f, 0.0f, 0.0f,
             -0.2f, 0.2f, 0.2f,
             0.0f, 0.0f, 0.0f,
             0.2f, 0.4f, 0.2f,
             0.0f, 0.0f, 0.0f,
             -0.2f, 0.4f, 0.2f,
             0.0f, 0.0f, 0.0f,
            };

            float[] armsVerts = {
                
             //Right Arm
             -0.5f,0.0f,0.2f,
             0.0f,0.0f,0.0f,
             -1.4f,-1.0f,0.2f,
             0.0f,0.0f,0.0f,
                
             //Left Arm
             0.5f,0.0f,0.2f,
             0.0f,0.0f,0.0f,
             1.4f,-1.0f,0.2f,
             0.0f,0.0f,0.0f,
            };
            float[] legsVerts = {
             // Legs (Using Line Strip)
             -0.4f, 1.5f, 0.2f,
             0.0f, 0.0f, 0.0f,
             -0.4f, 1.0f, 0.2f,
             0.0f, 0.0f, 0.0f,
              0.4f, 1.0f, 0.2f,
             0.0f, 0.0f, 0.0f,
             0.4f, 1.5f, 0.2f,
             0.0f, 0.0f, 0.0f
            };


            cheeseBufferID = GPU.GenerateBuffer(cheeseVerts);
            armsBufferID = GPU.GenerateBuffer(armsVerts);
            legsBufferID = GPU.GenerateBuffer(legsVerts);
            mouthBufferID = GPU.GenerateBuffer(mouthVerts);
            eyesBufferID = GPU.GenerateBuffer(eyesVerts);


            // View matrix 
            ViewMatrix = glm.lookAt(
            new vec3(0, 0, 4),// eye
            new vec3(0, 0, 0), // center
            new vec3(0, 1, 0)); // up

            // Model Matrix Initialization
            ModelMatrix = new mat4(1);

            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            ProjectionMatrix = glm.perspective(-45.0f, 3.0f / 4.0f, 1f, 100.0f);

            // Our MVP matrix which is a multiplication of our 3 matrices 
            sh.UseShader();


            //Get a handle for our "MVP" uniform (the holder we created in the vertex shader)
            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");
            useTextureID = Gl.glGetUniformLocation(sh.ID, "useTexture");
            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

            timer.Start();
        }

        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);


            #region Cheese
            Gl.glUniform1i(useTextureID, 0);
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, cheeseBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));
            tex1.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 12);


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            Gl.glDisableVertexAttribArray(2);
            #endregion
            
            #region Eyes
            Gl.glUniform1i(useTextureID, 1);
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
            Gl.glUniform1i(useTextureID, 1);
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
            Gl.glUniform1i(useTextureID, 1);
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
            Gl.glUniform1i(useTextureID, 1);
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
