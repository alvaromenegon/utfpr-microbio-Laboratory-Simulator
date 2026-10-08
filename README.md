### Projeto baseado no repositório original chamx2/Laboratory-Simulator - veja o README original ao final

# MicroBio Lab - Simulador de Laboratório Virtual de Microbiologia
Projeto desenvolvido como Trabalho de Conclusão de Curso de Alvaro Rosario no curso de Engenharia de Software pela UTFPR. O objetivo foi de criar um simulador que pudesse ser utilizado como complemento as aulas práticas em laboratórios de biologia e áreas afins no câmpus da Universidade.

Diferentemente do projeto original, que consistia de 9 aulas práticas fixadas no código, essa nova versão alterou completamente a dinâmica dos experimentos práticos, tornando-os totalmente dinâmicos, carregados através de dados recebidos por uma API Rest (disponibilizada em [alvaromenegon/microbio-lab](https://github.com/alvaromenegon/microbio-lab)). 

Para isso, foi implementando um sistema baseando-se nos conceitos de ECS (entity-component-system) para evitar manter qualquer dado *hardcoded*, deixando o código apenas como interpretador desses dados.

# Requisitos técnicos
Game Engine:
  - Unity 3D Version 6000.3.2f1
  
Sotfware de Modelagem 3D:
  - Blender
  
Linguagem dos Scripts Unity:
  - C#
  
Plataformas disponibilizadas:
  - Web (via WebGL)*
*Unity é multi-plataforma, mas toda a UI e scripts de movimentação foram desenvolvidos para utilização em navegadores 

Créditos ao autor original: [John Hay Supetran](https://github.com/chamx2/)

### Original README from chamx2/Laboratory-Simulator (en)
### README original de chamx2/Laboratory-Simulator (pt)

# Laboratory-Simulator
A virtual reality laboratory simulator. The simulation consist of 3 subjects, for physics, biology, and chemistry. Each subject has 3 different types of experiment that you have to complete before going to the next experiment of the selected subject.


# Technical Requirements
Game Engine:
  - Unity 3D Version 2017.4.14f1 LTS
  
3D Modelling Software:
  - Blender 3D
  
Scripting Language:
  - C#
  
Current Platform Availability:
  - Android
  - Google Cardboard
  
  
# Contents

Scene 1:
  - Laboratory Apparatus Scene 
    - This scene introduces some laboratory apparatus to the user. 
    
Scene 2:
  - Subject Selection Scene
    - User selects subject based on what type of experiments they want to conduct.

Scene 3:
  - Experiment scene
    - Each subject contains 3 experiments
    - How to conduct the experiment will be shown in the blackboard inside the scene. 
    - Upon finishing an experiment successfully, A portal to the next experiment will appear in the blackboard shortly.
    
# Game Instruction
  - Navigation
    - Since it is only available for Android Google Cardboard, The user will use its head as the cursor or mouse to navigate all over the place
    
  - Interaction
    - To interact with objects you can either:
      - Stare
      - Press the button of the cardboard located at the upper right. 


 
Note: The game can be navigated without cardboard, to interact you can just tap the screen.

# How to use

If you are new to unity please proceed on this section. 

You can see on the repository, The Asset folder, it contains all the project content that you will see inside your Unity 3D editor or interface. 

## Scripting

This uses C# Mono scripting which can be found in Assets/ Scripts, you can search Scripting on the search bar of the project window to see other Scripts easily.

## Scenes

Unity uses what we called scenes, which contains different contents per each and can be interwined together by using scripts. Scenes can be found in Assets/Scenes/

## 3D Models and other Assets

Can be found on Assets/Game Contents and Assets/ Models folder, unity has the built-in function of prefabricating contents for the developer to be easily replicate things inside the engine or editor, which can be found in Assets/Prefabs



