using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class ToolsController
    {
        public PrevAndCurrent<ToolHandle> toolHandle { get; private set; } = new(ToolHandle.Inspect);

        WorldController worldController;

        public BuildTool buildTool { get; }
        public InspectTool inspectTool { get; }
        public DestroyTool destroyTool { get; }
        public Dictionary<ToolHandle, ITool> toolsByHandle;
        ITool currentTool;

        public delegate void ToolEvent();
        public ToolEvent onToolChanged;

        public ToolsController(WorldController worldController)
        {
            this.worldController = worldController;

            buildTool = new BuildTool(worldController);
            inspectTool = new InspectTool(worldController);
            destroyTool = new DestroyTool(worldController);
            toolsByHandle = new() {
                { ToolHandle.Build, buildTool },
                { ToolHandle.Inspect, inspectTool },
                { ToolHandle.Destroy, destroyTool },
            };

            if (toolsByHandle.ContainsKey(toolHandle.current))
            {
                currentTool = toolsByHandle[toolHandle.current];
                currentTool.Setup();
            }
        }

        //
        // Lifecycle
        //
        public void OnUpdate()
        {
            currentTool?.OnUpdate();
        }

        //
        // Public interface
        // 
        public void OnLeftMouseUp()
        {
            currentTool?.OnLeftMouseUp();
        }

        public void OnRightMouseUp()
        {
            currentTool?.OnRightMouseUp();
        }

        public void SetTool(ToolHandle newTool)
        {
            toolHandle.Set(newTool);

            // Transition states
            if (toolHandle.HasChanged())
            {
                // tear down previous tool (if there was one)
                if (toolsByHandle.ContainsKey(toolHandle.prev))
                {
                    //
                    var previousTool = toolsByHandle[toolHandle.prev];
                    previousTool.Teardown();
                }

                // set up new tool (if there is one)
                if (toolsByHandle.ContainsKey(toolHandle.current))
                {
                    currentTool = toolsByHandle[toolHandle.current];
                    currentTool.Setup();
                }
                else
                {
                    currentTool = null;
                }

                onToolChanged?.Invoke();
            }
        }
    }
}