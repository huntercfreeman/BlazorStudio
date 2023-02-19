using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.AccountCase;
using Fluxor;

namespace BlazorStudio.ClassLib.FileSystem.Classes;

public class WebsiteEnvironmentProvider : IEnvironmentProvider
{
    private readonly IState<AccountState> _accountStateWrap;

    public WebsiteEnvironmentProvider(
        IState<AccountState> accountStateWrap)
    {
        _accountStateWrap = accountStateWrap;
    }

    public IAbsoluteFilePath RootDirectoryAbsoluteFilePath
    {
        get
        {
            var accountState = _accountStateWrap.Value;
            
            return new AbsoluteFilePath(
                accountState.GroupName + '/',
                true,
                this);
        }
    }

    public IAbsoluteFilePath HomeDirectoryAbsoluteFilePath => new AbsoluteFilePath(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        true,
        this);
    
    public char DirectorySeparatorChar => '/';
    public char AltDirectorySeparatorChar => '/';
    
    public string GetRandomFileName()
    {
        return Guid.NewGuid().ToString();
    }
}