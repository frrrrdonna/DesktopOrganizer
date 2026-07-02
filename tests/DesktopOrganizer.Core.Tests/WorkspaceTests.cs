using DesktopOrganizer.Core.Models;

namespace DesktopOrganizer.Core.Tests;

public sealed class WorkspaceTests
{
    [Fact]
    public void Workspace_Initializes_With_Default_Name_And_Empty_Groups()
    {
        var workspace = new Workspace();

        Assert.Equal("Default", workspace.Name);
        Assert.NotNull(workspace.Groups);
        Assert.Empty(workspace.Groups);
    }

    [Fact]
    public void FenceGroup_Initializes_With_Sensible_Defaults()
    {
        var group = new FenceGroup();

        Assert.NotEqual(Guid.Empty, group.Id);
        Assert.Equal("New Group", group.Name);
        Assert.Equal(280, group.Width);
        Assert.Equal(360, group.Height);
        Assert.False(group.IsCollapsed);
        Assert.Null(group.Portal);
        Assert.NotNull(group.Items);
        Assert.Empty(group.Items);
    }

    [Fact]
    public void FenceItem_Initializes_With_Defaults()
    {
        var item = new FenceItem();

        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal(string.Empty, item.DisplayName);
        Assert.Equal(string.Empty, item.Path);
        Assert.Equal(FenceItemType.Unknown, item.Type);
    }

    [Fact]
    public void Workspace_Can_Add_And_Remove_Groups()
    {
        var workspace = new Workspace();
        var group = new FenceGroup { Name = "Test" };

        workspace.Groups.Add(group);
        Assert.Single(workspace.Groups);

        workspace.Groups.Remove(group);
        Assert.Empty(workspace.Groups);
    }

    [Fact]
    public void FenceGroup_Can_Add_And_Remove_Items()
    {
        var group = new FenceGroup();
        var item = new FenceItem { DisplayName = "test.txt", Path = @"C:\test.txt", Type = FenceItemType.File };

        group.Items.Add(item);
        Assert.Single(group.Items);

        group.Items.Remove(item);
        Assert.Empty(group.Items);
    }
}
