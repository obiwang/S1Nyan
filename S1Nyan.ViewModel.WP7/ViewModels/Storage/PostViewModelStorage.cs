using Caliburn.Micro;

namespace S1Nyan.ViewModels.Storage
{
    public class PostViewModelStorage : StorageHandler<PostViewModel>
    {
        public override void Configure()
        {
            Property(x => x.Title).InPhoneState().RestoreAfterActivation();
            Property(x => x.Tid).InPhoneState().RestoreAfterActivation();
            Property(x => x.CurrentPage).InPhoneState().RestoreAfterActivation();
            Property(x => x.ReplyText).InPhoneState().RestoreAfterActivation();
        }
    }
}