using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public partial class GridInitialization : SystemBase
{
    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var entityManager = World.EntityManager;
        var entities = entityManager.GetAllEntities(Allocator.Temp);
        Entity PlaceholderPrefab = Entity.Null;
        Entity NodePrefab = Entity.Null;
        foreach (var entity in entities)
        {
            if (entityManager.GetName(entity).Equals("Placeholder"))
            {
                PlaceholderPrefab = entity;
            } else if (entityManager.GetName(entity).Equals("Node"))
            {
                NodePrefab = entity;
            }
        }

        if (PlaceholderPrefab == Entity.Null || NodePrefab == Entity.Null) 
        {
            UnityEngine.Debug.Log("I can't find a prefab.");
        }

        var matrix = new Dictionary<int, Dictionary<int, Entity>>();
        var matrixPieces = new Dictionary<int, Dictionary<int, Entity>>();
        /**
         * Create the nodes of the grid and the pieces in the matrixes
         */
        for (int i = 0; i < 10; i++)
        {
            matrix.Add(i, new Dictionary<int, Entity>());
            matrixPieces.Add(i, new Dictionary<int, Entity>());
            for (int j = 0; j < 10; j++)
            {
                var newNode = ecb.Instantiate(NodePrefab);
                matrix[i].Add(j, newNode);
                if (i % 2 == 0)
                {
                    var newPiece = ecb.Instantiate(PlaceholderPrefab);
                    matrixPieces[i].Add(j, newPiece);
                } else
                {
                    matrixPieces[i].Add(j, Entity.Null);
                }
            }
        }

        /**
         * Link the nodes of the grid and each piece with the corresponding node (in both senses) in the matrixes
         */
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var nodeComponentData = new NodeComponentData
                {
                    Bottom = Entity.Null,
                    Left = Entity.Null,
                    Right = Entity.Null,
                    Up = Entity.Null
                };
                if (i - 1 >= 0)
                {
                    nodeComponentData.Up = matrix[i - 1][j];
                }
                if (i + 1 <= 9)
                {
                    nodeComponentData.Bottom = matrix[i + 1][j];
                }
                if (j - 1 >= 0)
                {
                    nodeComponentData.Left = matrix[i][j - 1];
                }
                if (j + 1 <= 9)
                {
                    nodeComponentData.Right = matrix[i][j + 1];
                }
                nodeComponentData.Node = matrixPieces[i][j];
                ecb.SetComponent(matrix[i][j], nodeComponentData);

                if (matrixPieces[i][j] != Entity.Null)
                {
                    ecb.SetComponent(matrixPieces[i][j], new PieceComponentData
                    {
                        Row = i,
                        Column = j,
                        Node = matrix[i][j]
                    });
                }
            }
        }

        /**
         * Spread the pieces and the nodes in the grid
         */
        var pivot = new Translation
        {
            Value = new float3(0, 0, 0)
        };

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                ecb.SetComponent(matrix[i][j], pivot);
                if (matrixPieces[i][j] != Entity.Null)
                {
                    ecb.SetComponent(matrixPieces[i][j], pivot);
                }
                pivot.Value += new float3(1.5f, 0, 0);
            }
            pivot.Value.x = 0;
            pivot.Value.y -= 1.5f;
        }

        ecb.Playback(entityManager);

        entityManager.SetEnabled(PlaceholderPrefab, false);
        Enabled = false;

        World.GetExistingSystem<MovementSystem>().Enabled = true;
        World.GetExistingSystem<SplineMovementSystem>().Enabled = true;
    }
    protected override void OnUpdate()
    {
        
    }
}
