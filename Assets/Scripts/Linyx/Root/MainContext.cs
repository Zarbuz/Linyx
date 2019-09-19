using Linyx.Controllers.Background;
using Linyx.Controllers.Bottom;
using Linyx.Controllers.Brush;
using Linyx.Controllers.Camera;
using Linyx.Controllers.Edit;
using Linyx.Controllers.Header;
using Linyx.Controllers.InitServices;
using Linyx.Controllers.Music;
using Linyx.Controllers.Project;
using Linyx.Controllers.Start;
using Linyx.Controllers.ViewManager;
using Linyx.Models.Line;
using Linyx.Services.Audio;
using Linyx.Services.Background;
using Linyx.Services.Brush;
using Linyx.Services.Camera;
using Linyx.Services.Curve;
using Linyx.Services.Draw;
using Linyx.Services.Project;
using Linyx.Services.Recording;
using Linyx.Services.SaveManager;
using Linyx.Services.Shortcut;
using Linyx.Services.ViewManager;
using Linyx.Views.Bottom;
using Linyx.Views.Brush;
using Linyx.Views.Edit;
using Linyx.Views.Header;
using Linyx.Views.Music;
using Linyx.Views.Project;
using Linyx.Views.Record;
using Linyx.Views.Render;
using Linyx.Views.Utils;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace Linyx.Root
{
    public class MainContext : MVCSContext
    {
        public MainContext() : base()
        {
        }

        public MainContext(MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
        {
        }

        public override IContext Start()
        {
            base.Start();

            StartSignal startSignal = injectionBinder.GetInstance<StartSignal>() as StartSignal;
            startSignal.Dispatch();

            return this;
        }

        protected override void mapBindings()
        {
            MapModels();
            MapServices();
            MapControllers();
            MapViews();
        }

        protected override void addCoreComponents()
        {
            base.addCoreComponents();

            injectionBinder.Unbind<ICommandBinder>();
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
        }

        private void MapViews()
        {
            mediationBinder.Bind<BrushView>().To<BrushMediator>();
            mediationBinder.Bind<HeaderView>().To<HeaderMediator>();
            mediationBinder.Bind<RenderView>().To<RenderMediator>();
            mediationBinder.Bind<ProjectView>().To<ProjectMediator>();
            mediationBinder.Bind<EditView>().To<EditMediator>();
            mediationBinder.Bind<MusicView>().To<MusicMediator>();
            mediationBinder.Bind<BottomView>().To<BottomMediator>();
            mediationBinder.Bind<UtilsView>().To<UtilsMediator>();
            mediationBinder.Bind<RecordView>().To<RecordMediator>();
        }

        private void MapModels()
        {
            injectionBinder.Bind<ILineModel>().To<LineModel>();
        }

        private void MapServices()
        {
            #region Audio
            AudioPeerService audioPeerService = (contextView as GameObject)?.GetComponentInChildren<AudioPeerService>();
            if (audioPeerService != null)
            {
                injectionBinder.Bind<IAudioPeerService>().ToValue(audioPeerService);
            }
            #endregion

            #region Brush
            BrushService brushService = (contextView as GameObject)?.GetComponentInChildren<BrushService>();
            if (brushService != null)
            {
                injectionBinder.Bind<IBrushService>().ToValue(brushService);
            }
            #endregion

            #region Camera

            CameraService cameraService = (contextView as GameObject)?.GetComponentInChildren<CameraService>();
            if (cameraService != null)
            {
                injectionBinder.Bind<ICameraService>().ToValue(cameraService);
            }

            #endregion

            #region MyRegion

            CurveService curveService = (contextView as GameObject)?.GetComponentInChildren<CurveService>();
            if (curveService != null)
            {
                injectionBinder.Bind<ICurveService>().ToValue(curveService);
            }

            #endregion

            #region Draw

            DrawService drawService = (contextView as GameObject)?.GetComponentInChildren<DrawService>();
            if (drawService != null)
            {
                injectionBinder.Bind<IDrawService>().ToValue(drawService);
            }

            #endregion

            #region ViewManager

            ViewManager viewManager = (contextView as GameObject)?.GetComponentInChildren<ViewManager>();
            if (viewManager != null)
            {
                injectionBinder.Bind<IViewManager>().ToValue(viewManager);
            }

            #endregion

            #region Background

            BackgroundService backgroundService = (contextView as GameObject)?.GetComponentInChildren<BackgroundService>();
            if (cameraService != null)
            {
                injectionBinder.Bind<IBackgroundService>().ToValue(backgroundService);
            }

            #endregion

            #region Project

            ProjectService projectService = (contextView as GameObject)?.GetComponentInChildren<ProjectService>();
            if (cameraService != null)
            {
                injectionBinder.Bind<IProjectService>().ToValue(projectService);
            }

            #endregion

            #region Shortcut

            ShortcutService shortcutService = (contextView as GameObject)?.GetComponentInChildren<ShortcutService>();
            if (shortcutService != null)
            {
                injectionBinder.Bind<IShortcutService>().ToValue(shortcutService);
            }

            #endregion

            #region Save

            SaveManager saveManager = (contextView as GameObject)?.GetComponentInChildren<SaveManager>();
            if (shortcutService != null)
            {
                injectionBinder.Bind<ISaveManager>().ToValue(saveManager);
            }

            #endregion

            #region Recording

            RecordingService recordingService = (contextView as GameObject)?.GetComponentInChildren<RecordingService>();
            if (recordingService != null)
            {
                injectionBinder.Bind<IRecordingService>().ToValue(recordingService);
            }

            #endregion
        }

        private void MapControllers()
        {
            MapCommands();
            MapSingleton();
        }

        private void MapCommands()
        {
            commandBinder.Bind<ToggleDrawModeSignal>().To<ToggleDrawModeCommand>();
            commandBinder.Bind<SaveProjectSignal>().To<SaveProjectCommand>();
            commandBinder.Bind<SaveProjectAsSignal>().To<SaveProjectAsCommand>();
            commandBinder.Bind<UndoSignal>().To<UndoCommand>();
            commandBinder.Bind<RedoSignal>().To<RedoCommand>();
            commandBinder.Bind<LoadProjectSignal>().To<LoadProjectCommand>();
            commandBinder.Bind<NewProjectSignal>().To<NewProjectCommand>();

            commandBinder.Bind<ChangeViewSignal>().To<ChangeViewCommand>();
            commandBinder.Bind<ToggleViewSignal>().To<ToggleViewCommand>();

            commandBinder.Bind<SetRenderModeSignal>().To<SetRenderModeCommand>();
            commandBinder.Bind<SetAngleValueSignal>().To<SetAngleValueCommand>();
            commandBinder.Bind<SetRadiusValueSignal>().To<SetRadiusValueCommand>();
            commandBinder.Bind<SetNumberValueSignal>().To<SetNumberValueCommand>();
            commandBinder.Bind<SetChromaticValueSignal>().To<SetChromaticValueCommand>();
            commandBinder.Bind<SetVignetteValueSignal>().To<SetVignetteValueCommand>();
            commandBinder.Bind<SetBloomValueSignal>().To<SetBloomValueCommand>();
            commandBinder.Bind<SetCameraRotationSignal>().To<SetCameraRotationCommand>();
            commandBinder.Bind<SetCameraPositionSignal>().To<SetCameraPositionCommand>();
            commandBinder.Bind<SetCameraRotationSpeedSignal>().To<SetCameraRotationSpeedCommand>();
            commandBinder.Bind<RecenterCameraSignal>().To<RecenterCameraCommand>();
            commandBinder.Bind<SetKaleidoscopeEffectSignal>().To<SetKaleidoscopeEffectCommand>();

            commandBinder.Bind<SetBrushWidthCurveSignal>().To<SetBrushWidthCurveCommand>();
            commandBinder.Bind<SetBrushEmissionIntensitySignal>().To<SetBrushEmissionIntensityCommand>();
            commandBinder.Bind<SetBrushEmissionValueSignal>().To<SetBrushEmissionValueCommand>();
            commandBinder.Bind<SetCenterXValueSignal>().To<SetCenterXValueCommand>();
            commandBinder.Bind<SetCenterYValueSignal>().To<SetCenterYValueCommand>();
            commandBinder.Bind<SetBrushShapeIndexSignal>().To<SetBrushShapeIndexCommand>();
            commandBinder.Bind<SetBrushShapeInitSizeSignal>().To<SetBrushShapeInitSizeCommand>();
            commandBinder.Bind<SetBrushGradientSignal>().To<SetBrushGradientCommand>();
            commandBinder.Bind<SetBrushEmissionColorSignal>().To<SetBrushEmissionColorCommand>();
            commandBinder.Bind<SetBrushFromCopySignal>().To<SetBrushFromCopyCommand>();
            commandBinder.Bind<ResetBrushSettingsSignal>().To<ResetBrushSettingsCommand>();

            commandBinder.Bind<SetTopBackgroundColorSignal>().To<SetTopBackgroundColorCommand>();
            commandBinder.Bind<SetBottomBackgroundColorSignal>().To<SetBottomBackgroundColorCommand>();
            commandBinder.Bind<SetIntensityBackgroundSignal>().To<SetIntensityBackgroundCommand>();
            commandBinder.Bind<SetExponentBackgroundSignal>().To<SetExponentBackgroundCommand>();
            commandBinder.Bind<SetDirectionXAngleSignal>().To<SetDirectionXAngleCommand>();
            commandBinder.Bind<SetDirectionYAngleSignal>().To<SetDirectionYAngleCommand>();

            commandBinder.Bind<SelectLineSignal>().To<SelectLineCommand>();
            commandBinder.Bind<UnselectLineSignal>().To<UnselectLineCommand>();
            commandBinder.Bind<DeleteLineSignal>().To<DeleteLineCommand>();
            commandBinder.Bind<UpdateLineSignal>().To<UpdateLineCommand>();
            commandBinder.Bind<UpdateEmissionColorSignal>().To<UpdateEmissionColorCommand>();
            commandBinder.Bind<UpdateCurveSignal>().To<UpdateCurveCommand>();
            commandBinder.Bind<UpdateGradientSignal>().To<UpdateGradientCommand>();

            commandBinder.Bind<LoadSongSignal>().To<LoadSongCommand>();

            commandBinder.Bind<ScreenshotSignal>().To<ScreenshotCommand>();
            commandBinder.Bind<ExportVideoSignal>().To<ExportVideoCommand>();

            commandBinder.Bind<InitServicesSignal>().To<InitServicesCommand>();
            commandBinder.Bind<StartSignal>().To<StartCommand>().Once().InSequence();
        }

        private void MapSingleton()
        {
            injectionBinder.Bind<AddLineSignal>().ToSingleton();
            injectionBinder.Bind<TopBackgroundColorSelectedSignal>().ToSingleton();
            injectionBinder.Bind<BottomBackgroundColorSelectedSignal>().ToSingleton();
            injectionBinder.Bind<SongLoadedSignal>().ToSingleton();
            injectionBinder.Bind<SetInfoBarTextSignal>().ToSingleton();
            injectionBinder.Bind<FullCleanupSignal>().ToSingleton();
            injectionBinder.Bind<UndoAvailableSignal>().ToSingleton();
            injectionBinder.Bind<RedoAvailableSignal>().ToSingleton();
            injectionBinder.Bind<SaveAvailableSignal>().ToSingleton();
            injectionBinder.Bind<LineDeletedSignal>().ToSingleton();
            injectionBinder.Bind<LineAddedSignal>().ToSingleton();
            injectionBinder.Bind<LineUpdatedSignal>().ToSingleton();
            injectionBinder.Bind<ToggleRulerCameraSignal>().ToSingleton();
            injectionBinder.Bind<NewCameraPositionSignal>().ToSingleton();
            injectionBinder.Bind<BrushGradientSelectedSignal>().ToSingleton();
            injectionBinder.Bind<BrushEmissionColorSelectedSignal>().ToSingleton();
            injectionBinder.Bind<UpdateEmissionColorSelectedSignal>().ToSingleton();
            injectionBinder.Bind<UpdateGradientSelectedSignal>().ToSingleton();
            injectionBinder.Bind<SetBrushFromCopySelectedSignal>().ToSingleton();
            injectionBinder.Bind<SongStartedSignal>().ToSingleton();
            injectionBinder.Bind<SongStoppedSignal>().ToSingleton();
        }
    }
}
