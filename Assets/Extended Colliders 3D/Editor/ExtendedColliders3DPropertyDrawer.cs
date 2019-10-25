using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ExtendedColliders3D.ExtendedCollders3DProperties))]
public class ExtendedColliders3DPropertyDrawer : PropertyDrawer {

    //Constants.
    const int circleIndex = 0;
    const int circleHalfIndex = 1;
    const int coneIndex = 2;
    const int coneHalfIndex = 3;
    const int cubeIndex = 4;
    const int cylinderIndex = 5;
    const int cylinderHalfIndex = 6;
    const int quadIndex = 7;
    const int triangleIndex = 8;
    const int fixedRows = 20;
    static readonly int[] rows = { 2, 2, 2, 3, 6, 7, 8, 1, 1 };

    //Get property height.
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        for (int i = 0; i < 4; i++)
            property.Next(i == 0);
        return (base.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing) * (rows[property.intValue] + fixedRows);
    }

    //On GUI.
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        //Set the row height and initial position.
        float rowHeight = base.GetPropertyHeight(property, label);
        position = new Rect(position.xMin, position.yMin, position.width, rowHeight);
        rowHeight += EditorGUIUtility.standardVerticalSpacing;

        //Mesh collider settings.
        addRow(ref position, rowHeight);
        EditorGUI.LabelField(position, "Mesh Collider Settings", EditorStyles.boldLabel);
        property.Next(true);
        addRow(ref position, rowHeight);
        EditorGUI.BeginProperty(position, label, property);
        property.boolValue = EditorGUI.Toggle(position, new GUIContent("Convex", "Is this collider convex?"), property.boolValue);
        bool convex = property.boolValue;
        EditorGUI.EndProperty();
        property.Next(false);
        addRow(ref position, rowHeight);
        EditorGUI.indentLevel = 1;
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginDisabledGroup(!convex);
        if (!convex)
            property.boolValue = false;
        property.boolValue = EditorGUI.Toggle(position, new GUIContent("Is Trigger", "Is this collider a trigger? Triggers are only supported on convex " +
                "colliders."), property.boolValue);
        EditorGUI.EndDisabledGroup();
        EditorGUI.EndProperty();
        EditorGUI.indentLevel = 0;
        property.Next(false);
        addRow(ref position, rowHeight);
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(position, property, new GUIContent("Material"), false);
        EditorGUI.EndProperty();

        //General collider settings.
        addRow(ref position, rowHeight);
        addRow(ref position, rowHeight);
        EditorGUI.LabelField(position, "General Collider Settings", EditorStyles.boldLabel);
        property.Next(false);
        addRow(ref position, rowHeight);
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(position, property, new GUIContent("Collider Type", "The type of 3D primitive shape to create."), false);
        int colliderType = property.intValue;
        EditorGUI.EndProperty();
        property.Next(false);
        addRow(ref position, rowHeight);
        EditorGUI.BeginProperty(position, label, property);
        property.vector3Value = EditorGUI.Vector3Field(position, new GUIContent("Centre", "The centre point of the collider."), property.vector3Value);
        EditorGUI.EndProperty();
        addRow(ref position, rowHeight);
        property.Next(false);
        addRow(ref position, rowHeight);
        EditorGUI.BeginProperty(position, label, property);
        property.vector3Value = EditorGUI.Vector3Field(position, new GUIContent("Rotation", "The rotation of the collider, represented as Euler angles."),
                property.vector3Value);
        EditorGUI.EndProperty();
        addRow(ref position, rowHeight);
        property.Next(false);
        addRow(ref position, rowHeight);
        EditorGUI.BeginProperty(position, label, property);
        property.vector3Value = EditorGUI.Vector3Field(position, new GUIContent("Size", "The scale of the collider."), property.vector3Value);
        EditorGUI.EndProperty();
        addRow(ref position, rowHeight);
        property.Next(false);
        addRow(ref position, rowHeight);
        EditorGUI.BeginProperty(position, label, property);
        property.boolValue = EditorGUI.Toggle(position, new GUIContent("Flip Faces", "Whether to flip the collider faces. Doing so turns the collider inside-" +
                "out, meaning objects will collide with, for example, the inside of cylinder instead of the outside."), property.boolValue);
        EditorGUI.EndProperty();

        //Collider-specific settings.
        addRow(ref position, rowHeight);
        addRow(ref position, rowHeight);
        EditorGUI.LabelField(position, new string[] { "Circle", "Circle - Half", "Cone", "Cone - Half", "Cube", "Cylinder", "Cylinder - Half", "Quad",
                "Triangle" }[colliderType] + " Settings", EditorStyles.boldLabel);
        property.Next(false);
        if (colliderType == circleIndex || colliderType == circleHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.intValue = EditorGUI.IntSlider(position, new GUIContent("Vertices", "The number of vertices the circle will have. More vertices means a " +
                    "smoother circle."), property.intValue, 3, 128);
            EditorGUI.EndProperty();
        }
        property.Next(false);
        if (colliderType == circleIndex || colliderType == circleHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.boolValue = EditorGUI.Toggle(position, new GUIContent("Two-Sided", "Whether the circle should allow collisions from both sides."),
                    property.boolValue);
            EditorGUI.EndProperty();
        }
        property.Next(false);
        if (colliderType == coneIndex || colliderType == coneHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.intValue = EditorGUI.IntSlider(position, new GUIContent("Faces", "The number of faces the cone will have. More faces means a smoother " +
                    "surface."), property.intValue, 3, 128);
            EditorGUI.EndProperty();
        }
        property.Next(false);
        if (colliderType == coneIndex || colliderType == coneHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.boolValue = EditorGUI.Toggle(position, new GUIContent("Cap", "Whether to add a cap onto the bottom of the cone."), property.boolValue);
            EditorGUI.EndProperty();
        }
        property.Next(false);
        if (colliderType == coneHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.boolValue = EditorGUI.Toggle(position, new GUIContent("Cap Flat End", "Whether to cap the flat side of the half cone."),
                    property.boolValue);
            EditorGUI.EndProperty();
        }
        for (int i = 0; i < 6; i++) {
            property.Next(false);
            if (colliderType == cubeIndex) {
                addRow(ref position, rowHeight);
                EditorGUI.BeginProperty(position, label, property);
                string faceName = new string[] { "Top", "Bottom", "Left", "Right", "Forward", "Back" }[i];
                property.boolValue = EditorGUI.Toggle(position, new GUIContent(faceName + " Face", "Whether the cube should have its " + faceName.ToLower() +
                        " face."), property.boolValue);
                EditorGUI.EndProperty();
            }
        }
        property.Next(false);
        if (colliderType == cylinderIndex || colliderType == cylinderHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.intValue = EditorGUI.IntSlider(position, new GUIContent("Faces", "The number of faces the cylinder will have. More faces mean a " +
                    "smoother surface."), property.intValue, 3, 128);
            EditorGUI.EndProperty();
        }
        property.Next(false);
        if (colliderType == cylinderIndex || colliderType == cylinderHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.boolValue = EditorGUI.Toggle(position, new GUIContent("Cap Top", "Whether to add a cap onto the top of the cylinder."),
                    property.boolValue);
            EditorGUI.EndProperty();
        }
        property.Next(false);
        if (colliderType == cylinderIndex || colliderType == cylinderHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.boolValue = EditorGUI.Toggle(position, new GUIContent("Cap Bottom", "Whether to add a cap onto the bottom of the cylinder."),
                    property.boolValue);
            EditorGUI.EndProperty();
        }
        property.Next(false);
        if (colliderType == cylinderIndex || colliderType == cylinderHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.vector2Value = EditorGUI.Vector2Field(position, new GUIContent("Taper Top", "The amount to \"taper\" the top of the cylinder. Tapering " +
                    "stretches the cylinder to allow the top and bottom to have a different radii, to create, for example, a funnel shape."),
                    property.vector2Value);
            EditorGUI.EndProperty();
            addRow(ref position, rowHeight);
        }
        property.Next(false);
        if (colliderType == cylinderIndex || colliderType == cylinderHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.vector2Value = EditorGUI.Vector2Field(position, new GUIContent("Taper Bottom", "The amount to \"taper\" the bottom of the cylinder. " +
                    "Tapering stretches the cylinder to allow the top and bottom to have a different radii, to create, for example, a funnel shape."),
                    property.vector2Value);
            EditorGUI.EndProperty();
            addRow(ref position, rowHeight);
        }
        property.Next(false);
        if (colliderType == cylinderHalfIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.boolValue = EditorGUI.Toggle(position, new GUIContent("Cap Flat End", "Whether to cap the flat side of the half cylinder."),
                    property.boolValue);
            EditorGUI.EndProperty();
        }
        property.Next(false);
        if (colliderType == quadIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.boolValue = EditorGUI.Toggle(position, new GUIContent("Two-Sided", "Whether the quad should allow collisions from both sides."),
                    property.boolValue);
            EditorGUI.EndProperty();
        }
        property.Next(false);
        if (colliderType == triangleIndex) {
            addRow(ref position, rowHeight);
            EditorGUI.BeginProperty(position, label, property);
            property.boolValue = EditorGUI.Toggle(position, new GUIContent("Two-Sided", "Whether the triangle should allow collisions from both sides."),
                    property.boolValue);
            EditorGUI.EndProperty();
        }

        //Editor settings.
        addRow(ref position, rowHeight);
        addRow(ref position, rowHeight);
        EditorGUI.LabelField(position, "Editor Settings", EditorStyles.boldLabel);
        property.Next(false);
        addRow(ref position, rowHeight);
        EditorGUI.BeginProperty(position, label, property);
        property.colorValue = EditorGUI.ColorField(position, new GUIContent("Colour", "The colour to draw the collider gizmo in the editor."),
                property.colorValue);
        EditorGUI.EndProperty();
    }

    //Add a row to the inspector.
    void addRow(ref Rect position, float rowHeight) {
        position = new Rect(position.xMin, position.yMin + rowHeight, position.width, position.height);
    }
}
