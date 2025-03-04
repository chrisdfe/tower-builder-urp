using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerBuilder
{
    public class RoomTileSegmentTransformWrapper
    {
        public RoomTileSegment segment { get; private set; }
        public Transform segmentRootTransform { get; private set; }
        public Dictionary<string, GameObject> variantMap { get; private set; }
        public string activeVariant { get; private set; }
        public GameObject activeVariantGameObject { get; private set; }

        RoomTileSegmentDefinition definition;

        public RoomTileSegmentTransformWrapper(RoomTileSegment segment, Transform roomTileRootTransform)
        {
            this.segment = segment;
            definition = RoomTileConstants.SEGMENT_DEFINITIONS[segment];

            activeVariant = definition.defaultVariant;
            segmentRootTransform = roomTileRootTransform.Find(definition.rootNodeName);
            variantMap = CreateSegmentVariantMap();

            SetActiveVariant(definition.defaultVariant);
        }

        //
        // public interface
        //
        public void SetColor(Color color)
        {
            foreach (var go in variantMap.Values)
            {
                var meshRenderer = go.GetComponent<MeshRenderer>();

                // TODO - recurse
                if (meshRenderer != null)
                {
                    meshRenderer.material.SetColor("_BaseColor", color);
                }
            }
        }

        public void SetHighlightIntensity(float intensity)
        {
            SetMaterialFloatValue("_HighlightIntensity", intensity);
        }

        public void SetValidBlueprintColorAmount(float amount)
        {
            SetMaterialFloatValue("_ValidBlueprintColorAmount", amount);
        }

        public void SetInvalidBlueprintColorAmount(float amount)
        {
            SetMaterialFloatValue("_InvalidBlueprintColorAmount", amount);
        }

        public void SetMarkedForDeletionColorAmount(float amount)
        {
            SetMaterialFloatValue("_MarkedForDeletionColorAmount", amount);
        }

        public void SetInteriorLightsIntensity(float amount)
        {
            SetMaterialFloatValue("_InteriorLightsIntensity", amount);
        }

        public void SetMaterial(Material material)
        {
            foreach (var go in variantMap.Values)
            {
                var meshRenderer = go.GetComponent<MeshRenderer>();

                // TODO - recurse
                if (meshRenderer != null)
                {
                    meshRenderer.material = material;
                }
            }
        }

        public void SetActive(bool isActive)
        {
            segmentRootTransform.gameObject.SetActive(isActive);
        }

        public void SetActiveVariant(string activeVariant)
        {
            this.activeVariant = activeVariant;

            foreach (var entry in variantMap)
            {
                var variantName = entry.Key;
                var variant = entry.Value;
                variant.SetActive(variantName == activeVariant);
            }
        }

        //
        // Private interface
        //
        void SetMaterialFloatValue(string inputName, float value)
        {
            foreach (var go in variantMap.Values)
            {
                var meshRenderer = go.GetComponent<MeshRenderer>();

                // TODO - recurse
                if (meshRenderer != null)
                {
                    meshRenderer.material.SetFloat(inputName, value);
                }
            }
        }

        Dictionary<string, GameObject> CreateSegmentVariantMap()
        {
            var result = new Dictionary<string, GameObject>();

            foreach (var variant in definition.variants)
            {
                var variantTransform = segmentRootTransform.Find($"{definition.rootNodeName}_{variant}");
                Assert.IsNotNull(variantTransform);

                var variantGameObject = variantTransform.gameObject;
                Assert.IsNotNull(variantGameObject);

                result.Add(variant, variantGameObject);
            }

            return result;
        }
    }
}