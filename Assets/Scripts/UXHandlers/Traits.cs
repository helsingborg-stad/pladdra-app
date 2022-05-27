using System.Linq;
using Lean.Common;
using Lean.Touch;
using UnityEngine;
using Utility;

namespace UXHandlers
{
    public static class Traits
    {
        public static readonly IUxHandlerTrait AllowTranslate = new UxHandlerTrait<LeanDragTranslateAlong>()
        {
            OnActivate = (c, ctx) => c.enabled = true,
            OnDeactivate = (c, ctx) => c.enabled = false
        };
        public static readonly IUxHandlerTrait AllowRotate = new UxHandlerTrait<LeanTwistRotateAxis>()
        {
            OnActivate = (c, ctx) => c.enabled = true,
            OnDeactivate = (c, ctx) => c.enabled = false
        };
        public static readonly IUxHandlerTrait AllowScale = new UxHandlerTrait<LeanPinchScale>()
        {
            OnActivate = (c, ctx) => c.enabled = true,
            OnDeactivate = (c, ctx) => c.enabled = false
        };
        public static readonly IUxHandlerTrait AllowSelect = new UxHandlerTrait<LeanSelectable>()
        {
            OnActivate = (c, ctx) =>
            {
                c.enabled = true;
                c.OnSelected.AddListener(leanSelect => ctx.Events.OnSelected(ctx.Scene, ctx.Workspace, ctx.GameObject));
                c.OnDeselected.AddListener(leanSelect => ctx.Events.OnDeselected(ctx.Scene, ctx.Workspace, ctx.GameObject));
            },
            OnDeactivate = (c, ctx) =>
            {
                c.enabled = false;
                c.OnSelected.RemoveAllListeners();
                c.OnDeselected.RemoveAllListeners();
            }
        };

        public static readonly IUxHandlerTrait AllowBoxCollider = new UxHandlerTrait<BoxCollider>()
        {
            OnActivate = (c, ctx) => c.enabled = true,
            OnDeactivate = (c, ctx) => c.enabled = false
        };

        public static readonly IUxHandlerTrait AllowFlexibleBounds = new UxHandlerTrait<FlexibleBounds>()
        {
            OnActivate = (c, ctx) =>
            {
                c.CalculateBoundsFromChildrenAndThen(ctx.GameObject, bounds =>
                {
                    ctx.TryConfigureComponent<BoxCollider>(boxCollider =>
                    {
                        boxCollider.center = bounds.center;
                        boxCollider.size = bounds.size;
                    });

                    ctx.TryConfigureComponent<MeshFilter>(meshFilter =>
                    {
                        meshFilter.mesh = new BoundingBoxFactory(bounds.size, bounds.center).CreateMesh();
                    });
                });
            }
        };

        public static readonly IUxHandlerTrait AllowOutline = new UxHandlerTrait<MeshRenderer>()
        {
            OnActivate = (c, ctx) =>
            {
                c.enabled = true;
                c.materials
                    .Where(material => material.shader.name == "Sprites/Outline")
                    .ToList()
                    .ForEach(material =>
                    {
                        material.SetColor("_Color", new Color(1f, 1f, 1f, 0.16f));
                        material.SetColor("_SolidOutline", new Color(1f, 1f, 1f, 0.66f));
                    });
            },
            OnDeactivate = (c, ctx) => c.enabled = false
        };

        public static readonly IUxHandlerTrait AllowOutlineSelected = new UxHandlerTrait<MeshRenderer>()
        {
            OnSelect = (c, ctx) =>
            {
                c.materials
                    .Where(material => material.shader.name == "Sprites/Outline")
                    .ToList()
                    .ForEach(material =>
                    {
                        material.SetColor("_Color", new Color(1f, 0f, 0f, 0.12f));
                        material.SetColor("_SolidOutline", new Color(1f, 0f, 0f, 0.66f));
                    });
            },
            OnDeselect = (c, ctx) =>
            {
                c.materials
                    .Where(material => material.shader.name == "Sprites/Outline")
                    .ToList()
                    .ForEach(material =>
                    {
                        material.SetColor("_Color", new Color(1f, 1f, 1f, 0.16f));
                        material.SetColor("_SolidOutline", new Color(1f, 1f, 1f, 0.66f));
                    });
            }
        };
    }
}