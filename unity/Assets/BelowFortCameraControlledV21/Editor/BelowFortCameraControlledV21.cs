#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace ElswordPixel.Editor.BelowFortCameraControlledV21 {
 [InitializeOnLoad] public static class Worker {
  const string Active="Elsword.BelowFortCameraComponent.Active",Pending="Elsword.BelowFortCameraComponent.Pending",Output="Elsword.BelowFortCameraComponent.Output",Bundle="Elsword.BelowFortCameraComponent.Bundle";static bool fault;static int phase,readyFrame;
  static Worker(){if(SessionState.GetBool(Active,false))Attach();}
  public static void Run(){if(!Application.isBatchMode||EditorApplication.isPlaying||EditorApplication.isPlayingOrWillChangePlaymode||EditorApplication.isCompiling)throw new InvalidOperationException("Idle batch Editor required");string output=Environment.GetEnvironmentVariable("EL_BELOW_FORT_CAMERA_OUTPUT"),bundle=Environment.GetEnvironmentVariable("EL_BELOW_FORT_CAMERA_BUNDLE");if(string.IsNullOrEmpty(output)||string.IsNullOrEmpty(bundle)||Directory.Exists(output))throw new InvalidOperationException("Fresh explicit camera fixture output required");Directory.CreateDirectory(output);SessionState.SetString(Output,output);SessionState.SetString(Bundle,bundle);SessionState.SetBool(Active,true);SessionState.SetBool(Pending,true);fault=false;Attach();EditorSceneManager.OpenScene(DemoBuilder.ScenePath);EditorApplication.isPlaying=true;}
  static void Attach(){EditorApplication.update-=Poll;EditorApplication.update+=Poll;Application.logMessageReceived-=Logged;Application.logMessageReceived+=Logged;}
  static void Logged(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)fault=true;}
  static void Poll(){if(fault){Finish(2);return;}if(!EditorApplication.isPlaying||EditorApplication.isCompiling)return;var scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>();if(!scene)return;try{string output=SessionState.GetString(Output,"");if(SessionState.GetBool(Pending,false)){SessionState.SetBool(Pending,false);scene.enabled=false;scene.BeginBelowFortCameraComponentFixtureV21(SessionState.GetString(Bundle,""),output);phase=0;readyFrame=Time.frameCount+2;return;}if(Time.frameCount<readyFrame)return;switch(phase){case 0:scene.CaptureBelowFortCameraComponentFixtureV21("fort-source-0s");scene.StepBelowFortCameraComponentFixtureV21(180);break;case 1:scene.CaptureBelowFortCameraComponentFixtureV21("fort-source-3s");scene.StepBelowFortCameraComponentFixtureV21(420);break;case 2:scene.CaptureBelowFortCameraComponentFixtureV21("fort-source-10s");scene.StepBelowFortCameraComponentFixtureV21(600);break;case 3:scene.CaptureBelowFortCameraComponentFixtureV21("fort-source-20s");scene.StepBelowFortCameraComponentFixtureV21(600);break;case 4:scene.CaptureBelowFortCameraComponentFixtureV21("fort-source-30s");var result=scene.FinishBelowFortCameraComponentFixtureV21();File.WriteAllText(Path.Combine(output,"report.json"),JsonUtility.ToJson(result,true));Finish(result.passed&&!fault?0:2);return;}phase++;readyFrame=Time.frameCount+2;}catch(Exception ex){File.WriteAllText(Path.Combine(SessionState.GetString(Output,""),"failure.txt"),ex.ToString());Debug.LogException(ex);Finish(2);}}

  static void Finish(int code){SessionState.SetBool(Active,false);SessionState.SetBool(Pending,false);EditorApplication.update-=Poll;Application.logMessageReceived-=Logged;EditorApplication.Exit(code);}
 }
}
#endif
