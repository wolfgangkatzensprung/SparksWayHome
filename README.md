# SparksWayHome

![Sal7zm](https://github.com/user-attachments/assets/f5a078a6-8d7c-409e-b66f-089dada4b7ea)

This is a cozy and somewhat puzzely Third-Person Adventure Platformer with a very simple core mechanic:

*Find button -> Press button -> Things happen!*

The idea behind this simple concept is to create level layouts that can be easily recontextualized in various creative ways.

This game was developed as a student project in 8 weeks at S4G School for Games in Berlin. It was released on [itch.io](https://s4g.itch.io/sparks-way-home) in August 2024.

## Responsibilities
- Gameplay Programming
- Editor Tools
- Music Creation
- Wwise Sound Implementation
- Technical Support for the team

## Highlights
The main challenge for me was to create a versatile game/level design toolkit that allows for very quick iteration while also providing an intuitive, modular workflow. This was achieved through several measures. 

- highly customizable [Player Movement](https://github.com/wolfgangkatzensprung/SparksWayHome/blob/main/Assets/_Player/Player%20Scripts/PlayerMovement.cs)
- a [Custom Editor](https://github.com/wolfgangkatzensprung/SparksWayHome/blob/main/Assets/Editor/SparksWayHomeEditorWindow.cs), specifically designed to match the needs of this project

![SparksWayHome CustomEditor](https://github.com/user-attachments/assets/cbd01d94-1077-4ad4-97f0-07cddfdaa75f)

- clearly visualized '[activatable](https://github.com/wolfgangkatzensprung/SparksWayHome/blob/main/Assets/_Level/Mechanics/Activatable.cs)' components such as a classic [Moving Platform](https://github.com/wolfgangkatzensprung/SparksWayHome/blob/main/Assets/_Level/Mechanics/MovingPlatform.cs) using animation curves, or a more versatile [MoveToTarget](https://github.com/wolfgangkatzensprung/SparksWayHome/blob/main/Assets/_Level/Mechanics/MoveToTarget.cs) component

![SparksWayHome_GizmosShowcase](https://github.com/user-attachments/assets/ec819713-c26c-4884-9558-7029fd30c342)

- and of course, the key element to proceeding in the world, the [Button](https://github.com/wolfgangkatzensprung/SparksWayHome/blob/main/Assets/_Level/Mechanics/ButtonScript.cs), featuring different modes of activation, displayed by a custom Inspector that only shows currently relevant settings

## Dependencies
- [LaFinca Midgard Skybox](https://assetstore.unity.com/packages/2d/textures-materials/sky/midgard-skybox-273733)
- [Grid Prototype Materials](https://assetstore.unity.com/packages/2d/textures-materials/grid-prototype-materials-214264) (to view the test levels)

![grafik](https://github.com/user-attachments/assets/1f06efff-0a03-415a-ac53-2e9ce7807b68)
