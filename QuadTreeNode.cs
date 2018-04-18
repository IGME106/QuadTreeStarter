using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

/// <summary>
/// IGME-106 - Game Development and Algorithmic Problem Solving
/// Practice Homework 4
/// Class Description   : Main program
/// Author              : Benjamin Kleynhans
/// Modified By         : Benjamin Kleynhans
/// Date                : March 26, 2018
/// Filename            : QuadTreeNode.cs
/// </summary>

namespace QuadTreeStarter
{
    class QuadTreeNode
    {
        #region Constants
        // The maximum number of objects in a quad
        // before a subdivision occurs
        private const int MAX_OBJECTS_BEFORE_SUBDIVIDE = 3;
        #endregion

        #region Variables
        // The game objects held at this level of the tree
        private List<GameObject> _objects;

        // This quad's rectangle area
        private Rectangle _rect;

        // This quad's divisions
        private QuadTreeNode[] _divisions;
        #endregion

        #region Properties
        /// <summary>
        /// The divisions of this quad
        /// </summary>
        public QuadTreeNode[] Divisions { get { return _divisions; } }

        /// <summary>
        /// This quad's rectangle
        /// </summary>
        public Rectangle Rectangle { get { return _rect; } }

        /// <summary>
        /// The game objects inside this quad
        /// </summary>
        public List<GameObject> GameObjects { get { return _objects; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Quad Tree
        /// </summary>
        /// <param name="x">This quad's x position</param>
        /// <param name="y">This quad's y position</param>
        /// <param name="width">This quad's width</param>
        /// <param name="height">This quad's height</param>
        public QuadTreeNode(int x, int y, int width, int height)
        {
            // Save the rectangle
            _rect = new Rectangle(x, y, width, height);

            // Create the object list
            _objects = new List<GameObject>();

            // No divisions yet
            _divisions = null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a game object to the quad.  If the quad has too many
        /// objects in it, and hasn't been divided already, it should
        /// be divided
        /// </summary>
        /// <param name="gameObj">The object to add</param>
        public void AddObject(GameObject gameObj)
        {
            // ACTIVITY: Complete this method

            bool objectAdded = false;

            if (this.Rectangle.Contains(gameObj.Rectangle))                                 // If the supplied rectangle fits in this rectangle
            {
                if ((this.GameObjects.Count < MAX_OBJECTS_BEFORE_SUBDIVIDE) && (this.Divisions == null))// if there are no divisions
                {                                                                           // and the maximum number of elements per node has not been reached
                    this.GameObjects.Add(gameObj);                                          // Add the passed object to this object
                    objectAdded = true;                                                     // Set delimiter to true (used if objects fall on the edge)
                }
                else
                {
                    if (this.Divisions == null)                                             // If there are no divisions
                    {
                        this.Divide();                                                      // Divide the rectangle
                    }

                    for (int i = 0; i < this.Divisions.Length; i++)                         // For each one of the divisions
                    {
                        if (this.Divisions[i].Rectangle.Contains(gameObj.Rectangle))        // If the passed object fits in a node
                        {
                            this.Divisions[i].AddObject(gameObj);                           // Add the object to the node
                            objectAdded = true;                                             // Set delimiter to true (used if objects fall on the edge)
                        }
                    }
                }
            }

            if (!objectAdded)                                                               // If the object has not been added at this point it falls on an 
            {                                                                               // edge, add it to the parent object that it fits into
                this.GameObjects.Add(gameObj);
            }
        }

        /// <summary>
        /// Divides this quad into 4 smaller quads.  Moves any game objects
        /// that are completely contained within the new smaller quads into
        /// those quads and removes them from this one.
        /// </summary>
        public void Divide()
        {
            // ACTIVITY: Complete this method
            if (this.Divisions == null)
            {
                this._divisions = new QuadTreeNode[4];                                      // Create a new Division
            }

            int subNodeWidth = (this.Rectangle.Width / 2);
            int subNodeHeight = (this.Rectangle.Height / 2);

            this.Divisions[0] = new QuadTreeNode(                                           // Assign sizes for each section
                                            this.Rectangle.X,
                                            this.Rectangle.Y,
                                            subNodeWidth,
                                            subNodeHeight
                                        );

            this.Divisions[1] = new QuadTreeNode(
                                            this.Rectangle.X + subNodeWidth,
                                            this.Rectangle.Y,
                                            subNodeWidth,
                                            subNodeHeight
                                        );

            this.Divisions[2] = new QuadTreeNode(
                                            this.Rectangle.X,
                                            this.Rectangle.Y + subNodeHeight,
                                            subNodeWidth,
                                            subNodeHeight
                                        );

            this.Divisions[3] = new QuadTreeNode(
                                            this.Rectangle.X + subNodeWidth,
                                            this.Rectangle.Y + subNodeHeight,
                                            subNodeWidth,
                                            subNodeHeight
                                        );

            List<GameObject> objectRemovalList = new List<GameObject>();                    // You cannot remove an object from a list while it is being
                                                                                            //  accessed, so the object is moved to this list for later removal
            foreach (GameObject localObject in GameObjects)                                 // Cycle through objects at this level
            {
                foreach (QuadTreeNode node in Divisions)                                    // Cycle through objects in each node
                {
                    if (node.Rectangle.Contains(localObject.Rectangle))                     // If the rectangle can be contained in the node
                    {
                        node.AddObject(localObject);                                        // Add the rectangle to the node
                        objectRemovalList.Add(localObject);                                 // Add it to the removal list
                    }
                }            
            }

            foreach (GameObject objectToRemove in objectRemovalList)
            {
                GameObjects.Remove(objectToRemove);                                         // Remove all objects in the removal list from GameObjects
            }

            objectRemovalList.Clear();                                                      // Clear the removal list
        }

        /// <summary>
        /// Recursively populates a list with all of the rectangles in this
        /// quad and any subdivision quads.  Use the "AddRange" method of
        /// the list class to add the elements from one list to another.
        /// </summary>
        /// <returns>A list of rectangles</returns>
        public List<Rectangle> GetAllRectangles()
        {
            List<Rectangle> rects = new List<Rectangle>();

            // ACTIVITY: Complete this method
            rects.Add(this.Rectangle);                                                      // Add the local rectangle to the list (this rectangle)

            if (this.Divisions != null)                                                     // If there are divisions (nodes)
            {
                for (int i = 0; i < this.Divisions.Length; i++)                             // Cycle through all the nodes
                {
                    rects.AddRange((IEnumerable<Rectangle>) this.Divisions[i].GetAllRectangles());// Return all the sub rectangles recursively
                }
            }

            return rects;
        }

        /// <summary>
        /// A possibly recursive method that returns the
        /// smallest quad that contains the specified rectangle
        /// </summary>
        /// <param name="rect">The rectangle to check</param>
        /// <returns>The smallest quad that contains the rectangle</returns>
        public QuadTreeNode GetContainingQuad(Rectangle rect)
        {
            QuadTreeNode returnValue = null;

            // ACTIVITY: Complete this method
            if (this.Rectangle.Contains(rect))                                              // If the passed object fits in this object
            {
                returnValue = this;                                                         // This object is currently the container

                if (this.Divisions != null)                                                 // If the object is divided (has nodes)
                {
                    foreach (QuadTreeNode thisNode in this.Divisions)                       // See if the passed object fits in any of the nodes
                    {
                        if (thisNode.Rectangle.Contains(rect))                              // If it does fit in a node
                        {
                            returnValue = thisNode.GetContainingQuad(rect);                 // Set the returnValue to the node (and call recursively)
                        }
                    }
                }
            }

            // Return null if this quad doesn't completely contain
            // the rectangle that was passed in
            return returnValue;
        }
        #endregion
    }
}
