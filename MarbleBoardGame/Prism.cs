using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MarbleBoardGame
{
    public class Prism : IDisposable
    {
        private RigidBody body;

        private BasicEffect[] effects;

        public Vector3 size;
        public Vector3 position;
        private VertexPositionNormalTexture[][] sideVerticies;
        private Texture2D[] textures;

        /// <summary>
        /// Creates a rectangular prism
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="body">Phyics body</param>
        /// <param name="textures">Textures</param>
        public Prism(GraphicsDevice device, RigidBody body, Texture2D[] textures)
        {
            this.body = body;

            //JVector jsize = ((BoxShape)body.Shape).Size;
            this.size = new Vector3(36);//jsize.X, jsize.Y, jsize.Z);
            this.position = new Vector3(body.Position.X, body.Position.Y, body.Position.Z);
            this.textures = textures;

            BuildShape();

            effects = new BasicEffect[6];
            for (int i = 0; i < 6; i++)
            {
                effects[i] = new BasicEffect(device);
                effects[i].TextureEnabled = true;
                effects[i].Texture = textures[i];
                effects[i].EnableDefaultLighting();
            }
        }

        /// <summary>
        /// Constructs the shapes texture vertices
        /// </summary>
        private void BuildShape()
        {
            sideVerticies = new VertexPositionNormalTexture[6][];

            // Calculate the position of the vertices on the top face.
            Vector3 topLeftFront = position + new Vector3(size.X, size.Y, size.Z);
            Vector3 topLeftBack = position + new Vector3(0, size.Y, size.Z);
            Vector3 topRightFront = position + new Vector3(size.X, size.Y, 0);
            Vector3 topRightBack = position + new Vector3(0, size.Y, 0);

            // Calculate the position of the vertices on the bottom face.
            Vector3 btmLeftFront = position + new Vector3(size.X, 0, size.Z);
            Vector3 btmLeftBack = position + new Vector3(0, 0, size.Z);
            Vector3 btmRightFront = position + new Vector3(size.X, 0, 0);
            Vector3 btmRightBack = position;

            // Normal vectors for each face (needed for lighting / display)
            Vector3 normalFront = new Vector3(0.0f, 0.0f, 1.0f) * size;
            Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f) * size;
            Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f) * size;
            Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f) * size;
            Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f) * size;
            Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f) * size;

            // UV texture coordinates
            Vector2 textureTopLeft = new Vector2(1.0f, 0.0f);
            Vector2 textureTopRight = new Vector2(0.0f, 0.0f);
            Vector2 textureBottomLeft = new Vector2(1.0f, 1.0f);
            Vector2 textureBottomRight = new Vector2(0.0f, 1.0f);

            // Add the vertices for the FRONT face.
            sideVerticies[0] = new VertexPositionNormalTexture[6];
            sideVerticies[0][0] = new VertexPositionNormalTexture(topLeftFront, normalFront, textureTopLeft);
            sideVerticies[0][1] = new VertexPositionNormalTexture(btmLeftFront, normalFront, textureBottomLeft);
            sideVerticies[0][2] = new VertexPositionNormalTexture(topRightFront, normalFront, textureTopRight);
            sideVerticies[0][3] = new VertexPositionNormalTexture(btmLeftFront, normalFront, textureBottomLeft);
            sideVerticies[0][4] = new VertexPositionNormalTexture(btmRightFront, normalFront, textureBottomRight);
            sideVerticies[0][5] = new VertexPositionNormalTexture(topRightFront, normalFront, textureTopRight);

            // Add the vertices for the BACK face.
            sideVerticies[1] = new VertexPositionNormalTexture[6];
            sideVerticies[1][0] = new VertexPositionNormalTexture(topLeftBack, normalBack, textureTopRight);
            sideVerticies[1][1] = new VertexPositionNormalTexture(topRightBack, normalBack, textureTopLeft);
            sideVerticies[1][2] = new VertexPositionNormalTexture(btmLeftBack, normalBack, textureBottomRight);
            sideVerticies[1][3] = new VertexPositionNormalTexture(btmLeftBack, normalBack, textureBottomRight);
            sideVerticies[1][4] = new VertexPositionNormalTexture(topRightBack, normalBack, textureTopLeft);
            sideVerticies[1][5] = new VertexPositionNormalTexture(btmRightBack, normalBack, textureBottomLeft);

            // Add the vertices for the TOP face.
            sideVerticies[2] = new VertexPositionNormalTexture[6];
            sideVerticies[2][0] = new VertexPositionNormalTexture(topLeftFront, normalTop, textureBottomLeft);
            sideVerticies[2][1] = new VertexPositionNormalTexture(topRightBack, normalTop, textureTopRight);
            sideVerticies[2][2] = new VertexPositionNormalTexture(topLeftBack, normalTop, textureTopLeft);
            sideVerticies[2][3] = new VertexPositionNormalTexture(topLeftFront, normalTop, textureBottomLeft);
            sideVerticies[2][4] = new VertexPositionNormalTexture(topRightFront, normalTop, textureBottomRight);
            sideVerticies[2][5] = new VertexPositionNormalTexture(topRightBack, normalTop, textureTopRight);

            // Add the vertices for the BOTTOM face. 
            sideVerticies[3] = new VertexPositionNormalTexture[6];
            sideVerticies[3][0] = new VertexPositionNormalTexture(btmLeftFront, normalBottom, textureTopLeft);
            sideVerticies[3][1] = new VertexPositionNormalTexture(btmLeftBack, normalBottom, textureBottomLeft);
            sideVerticies[3][2] = new VertexPositionNormalTexture(btmRightBack, normalBottom, textureBottomRight);
            sideVerticies[3][3] = new VertexPositionNormalTexture(btmLeftFront, normalBottom, textureTopLeft);
            sideVerticies[3][4] = new VertexPositionNormalTexture(btmRightBack, normalBottom, textureBottomRight);
            sideVerticies[3][5] = new VertexPositionNormalTexture(btmRightFront, normalBottom, textureTopRight);

            // Add the vertices for the LEFT face.
            sideVerticies[4] = new VertexPositionNormalTexture[6];
            sideVerticies[4][0] = new VertexPositionNormalTexture(topLeftFront, normalLeft, textureTopRight);
            sideVerticies[4][1] = new VertexPositionNormalTexture(btmLeftBack, normalLeft, textureBottomLeft);
            sideVerticies[4][2] = new VertexPositionNormalTexture(btmLeftFront, normalLeft, textureBottomRight);
            sideVerticies[4][3] = new VertexPositionNormalTexture(topLeftBack, normalLeft, textureTopLeft);
            sideVerticies[4][4] = new VertexPositionNormalTexture(btmLeftBack, normalLeft, textureBottomLeft);
            sideVerticies[4][5] = new VertexPositionNormalTexture(topLeftFront, normalLeft, textureTopRight);

            // Add the vertices for the RIGHT face. 
            sideVerticies[5] = new VertexPositionNormalTexture[6];
            sideVerticies[5][0] = new VertexPositionNormalTexture(topRightFront, normalRight, textureTopLeft);
            sideVerticies[5][1] = new VertexPositionNormalTexture(btmRightFront, normalRight, textureBottomLeft);
            sideVerticies[5][2] = new VertexPositionNormalTexture(btmRightBack, normalRight, textureBottomRight);
            sideVerticies[5][3] = new VertexPositionNormalTexture(topRightBack, normalRight, textureTopRight);
            sideVerticies[5][4] = new VertexPositionNormalTexture(topRightFront, normalRight, textureTopLeft);
            sideVerticies[5][5] = new VertexPositionNormalTexture(btmRightBack, normalRight, textureBottomRight);
        }

        /// <summary>
        /// Renders the rectangular prism
        /// </summary>
        /// <param name="device">Graphics device</param>
        public void Render(GraphicsDevice device, int size)
        {
            const float unitSize = 590f;
            float percentChange = size / unitSize;
            float percentAdd = 1 - percentChange;

            for (int i = 0; i < 6; i++)
            {
                if (body != null)
                {
                    effects[i].World = Matrix.CreateScale(0.175f * (size / unitSize)) * new Matrix(
                        body.Orientation.M11, body.Orientation.M12, body.Orientation.M13, 0,
                        body.Orientation.M21, body.Orientation.M22, body.Orientation.M23, 0,
                        body.Orientation.M31, body.Orientation.M32, body.Orientation.M33, 0,
                        0,                    0,                    0,                    1) *
                        Matrix.CreateTranslation(new Vector3(body.Position.X - (percentAdd * 80), body.Position.Y, body.Position.Z + (percentAdd * 80)));
                }

                effects[i].View = Matrix.CreateLookAt(new Vector3(10f, 60, 0f), new Vector3(0, 0, 0), Vector3.Up);
                effects[i].Projection = Matrix.CreateOrthographic(200, 200, 0.1f, 1000f);

                foreach (EffectPass pass in effects[i].CurrentTechnique.Passes)
                {
                    pass.Apply();
                    using (VertexBuffer buffer = new VertexBuffer(
                        device,
                        VertexPositionNormalTexture.VertexDeclaration,
                        6,
                        BufferUsage.WriteOnly))
                    {
                        buffer.SetData(sideVerticies[i]);
                        device.SetVertexBuffer(buffer);                      
                    }

                    device.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, sideVerticies[i], 0, 2);
                }
            }
        }

        /// <summary>
        /// Disposes the prism
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < 6; i++)
            {
                effects[i].Dispose();
            }
        }
    }
}
