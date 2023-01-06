using Fluxor;

namespace BlazorStudio.ClassLib.Store.PanelCase;

public partial record PanelsCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static PanelsCollection ReduceRegisterPanelModelAction(
            PanelsCollection previousPanelsCollection,
            RegisterPanelRecordAction registerPanelRecordAction)
        {
            if (previousPanelsCollection.PanelRecordsList.Any(
                    x => x.PanelRecordKey == registerPanelRecordAction.PanelRecord.PanelRecordKey))
            {
                return previousPanelsCollection;
            }

            var nextPanelsList = previousPanelsCollection.PanelRecordsList.Add(
                registerPanelRecordAction.PanelRecord);

            return previousPanelsCollection with
            {
                PanelRecordsList = nextPanelsList
            };
        }
        
        [ReducerMethod]
        public static PanelsCollection ReduceDisposePanelModelAction(
            PanelsCollection previousPanelsCollection,
            DisposePanelRecordAction disposePanelRecordAction)
        {
            var targetedPanelModel = previousPanelsCollection.PanelRecordsList.FirstOrDefault(
                    x => x.PanelRecordKey == disposePanelRecordAction.PanelRecordKey);

            if (targetedPanelModel is null)
                return previousPanelsCollection;

            var nextPanelsList = previousPanelsCollection.PanelRecordsList.Remove(
                targetedPanelModel);

            return previousPanelsCollection with
            {
                PanelRecordsList = nextPanelsList
            };
        }
        
        [ReducerMethod]
        public static PanelsCollection ReduceSetActivePanelTabAction(
            PanelsCollection previousPanelsCollection,
            SetActivePanelTabAction setActivePanelTabAction)
        {
            var targetedPanelModel = previousPanelsCollection.PanelRecordsList.FirstOrDefault(
                    x => x.PanelRecordKey == setActivePanelTabAction.PanelRecordKey);

            if (targetedPanelModel is null)
                return previousPanelsCollection;

            var nextPanelModel = targetedPanelModel with
            {
                ActivePanelTabKey = setActivePanelTabAction.PanelTabKey
            };

            var nextPanelsList = previousPanelsCollection.PanelRecordsList.Replace(
                targetedPanelModel,
                nextPanelModel);

            return previousPanelsCollection with
            {
                PanelRecordsList = nextPanelsList
            };
        }
    }
}