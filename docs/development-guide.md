# Desktop Organizer Development Guide

## 1. Project Goal

This project is a Windows desktop organizer inspired by Fences-style grouping.

Current scope:

- render group containers on a desktop-like surface;
- support local workspace persistence;
- prepare for file drag and drop, folder portals, and quick-launch behavior.

Out of scope for the current scaffold:

- replacing the Windows desktop process;
- Explorer shell extensions;
- automatic classification rules;
- cloud sync.

## 2. Solution Structure

```text
DesktopOrganizer.slnx
docs/
  development-guide.md
src/
  DesktopOrganizer.App/
    App.xaml
    App.xaml.cs
    DesktopOrganizer.App.csproj
    MainWindow.xaml
    MainWindow.xaml.cs
    Services/
      WorkspacePathProvider.cs
    ViewModels/
      BaseViewModel.cs
      FenceGroupViewModel.cs
      FenceItemViewModel.cs
      MainWindowViewModel.cs
    Views/
      FenceGroupView.xaml
      FenceGroupView.xaml.cs
  DesktopOrganizer.Core/
    DesktopOrganizer.Core.csproj
    Abstractions/
      ILauncher.cs
      IWorkspaceStore.cs
    Models/
      FenceGroup.cs
      FenceItem.cs
      FenceItemType.cs
      FolderPortal.cs
      Workspace.cs
  DesktopOrganizer.Infrastructure/
    DesktopOrganizer.Infrastructure.csproj
    Persistence/
      JsonWorkspaceStore.cs
    Shell/
      ShellLauncher.cs
tests/
  DesktopOrganizer.Core.Tests/
    DesktopOrganizer.Core.Tests.csproj
    WorkspaceTests.cs
```

## 3. Layer Responsibilities

### DesktopOrganizer.App

This is the WPF host application.

Files in this project should contain:

- XAML views;
- view models;
- UI-only services;
- startup composition code.

Files in this project should not contain:

- file persistence logic;
- shell integration logic;
- core business rules that can live outside WPF.

### DesktopOrganizer.Core

This is the domain layer.

Files in this project should contain:

- plain models;
- enums;
- interfaces;
- future application rules that do not depend on UI or Windows APIs.

Files in this project should not contain:

- WPF types;
- file IO implementation;
- Process launching code.

### DesktopOrganizer.Infrastructure

This is the implementation layer for external dependencies.

Files in this project should contain:

- JSON persistence;
- shell launching;
- future shortcut parsing;
- future folder watching;
- future startup registration.

### DesktopOrganizer.Core.Tests

This project contains focused tests around domain behavior.

Prefer testing:

- model invariants;
- future group manipulation logic;
- workspace save/load edge cases through small integration tests when needed.

## 4. File-by-File Implementation Rules

### src/DesktopOrganizer.App/App.xaml

Keep only application-level resources here.

Do:

- merged dictionaries;
- global styles;
- converters that are truly global.

Do not:

- put page-specific styles here unless reused;
- place startup logic in XAML.

### src/DesktopOrganizer.App/App.xaml.cs

This file is the temporary composition root.

Current responsibility:

- create the workspace store;
- create the main view model;
- load workspace data;
- show the main window.

Future code in this file should stay minimal. If startup grows, move construction into a dedicated bootstrapper or dependency injection setup.

### src/DesktopOrganizer.App/MainWindow.xaml

This is the shell surface for group rendering.

Current responsibility:

- host the collection of fence groups;
- provide layout surface for the initial desktop-like canvas.

When extending this file:

- keep it focused on layout;
- move each substantial group UI into a dedicated view;
- avoid putting event-heavy logic in code-behind.

### src/DesktopOrganizer.App/MainWindow.xaml.cs

Keep this code-behind minimal.

Allowed:

- WPF-only window event wiring;
- temporary interaction glue when no clean binding exists.

Avoid:

- persistence logic;
- list mutation logic;
- launch logic.

### src/DesktopOrganizer.App/Services/WorkspacePathProvider.cs

This file defines where the workspace JSON lives.

Rules:

- return a deterministic per-user path;
- do not mix persistence logic here;
- if path options grow later, replace this with a settings-aware service.

### src/DesktopOrganizer.App/ViewModels/BaseViewModel.cs

Base type for all view models.

Rules:

- keep it thin;
- only add shared UI state or helper behavior used by multiple view models.

### src/DesktopOrganizer.App/ViewModels/MainWindowViewModel.cs

This is the top-level UI coordinator.

Current responsibility:

- load the workspace;
- translate domain groups into view models;
- expose the root groups collection to the window.

Future features that belong here:

- create group command;
- save layout command;
- global search state;
- peek mode state.

Do not place:

- direct file IO;
- direct Process launching;
- drag/drop parsing code unless it is purely orchestration.

### src/DesktopOrganizer.App/ViewModels/FenceGroupViewModel.cs

This is the UI wrapper for one group container.

Current responsibility:

- mirror a `FenceGroup` model;
- expose group title and collapse state;
- expose group items as UI collection.

Future features that belong here:

- rename command;
- collapse command;
- drag reorder state;
- style settings for one group.

### src/DesktopOrganizer.App/ViewModels/FenceItemViewModel.cs

This is the UI wrapper for one item.

Keep it simple:

- expose display fields;
- later expose icon state and selection state.

Avoid putting launch behavior directly here unless the interaction is strictly item-scoped and still orchestrated through an abstraction.

### src/DesktopOrganizer.App/Views/FenceGroupView.xaml

This file owns the visual shape of a single group card.

Current responsibility:

- render title;
- render item list;
- provide a clean placeholder visual for future drag-and-drop and style work.

When expanding:

- keep styles local if used once;
- split item templates out only when complexity justifies it.

### src/DesktopOrganizer.App/Views/FenceGroupView.xaml.cs

Keep it empty unless WPF-specific interaction requires code-behind.

### src/DesktopOrganizer.Core/Abstractions/IWorkspaceStore.cs

This interface is the persistence boundary.

Any implementation must:

- load one workspace state;
- save one workspace state;
- avoid UI concerns.

### src/DesktopOrganizer.Core/Abstractions/ILauncher.cs

This interface is the shell-launch boundary.

Any implementation must:

- open a path or shortcut target;
- keep launch mechanics outside view models.

### src/DesktopOrganizer.Core/Models/Workspace.cs

Root aggregate for persisted layout.

Rules:

- keep it serialization-friendly;
- store only layout and grouping state;
- avoid UI-only properties.

### src/DesktopOrganizer.Core/Models/FenceGroup.cs

Represents one group container.

Current fields:

- identity;
- title;
- bounds;
- collapse state;
- optional folder portal;
- child items.

Future additions may include:

- visual theme;
- sort mode;
- lock state;
- z-index.

### src/DesktopOrganizer.Core/Models/FenceItem.cs

Represents one file, folder, or shortcut entry.

Rules:

- keep it focused on persisted item metadata;
- add icon cache references later only if needed;
- do not store transient WPF state here.

### src/DesktopOrganizer.Core/Models/FenceItemType.cs

Enum for item classification.

If new item types appear, add them here instead of using string literals.

### src/DesktopOrganizer.Core/Models/FolderPortal.cs

Represents a folder-backed group.

Future additions may include:

- sort order;
- filter configuration;
- refresh behavior.

### src/DesktopOrganizer.Infrastructure/Persistence/JsonWorkspaceStore.cs

This is the first persistence implementation.

Rules for future edits:

- keep serialization tolerant of missing files;
- centralize serializer options here;
- do not put unrelated business logic here.

If migration/versioning is added later, this is the first place to introduce it.

### src/DesktopOrganizer.Infrastructure/Shell/ShellLauncher.cs

This is the first shell execution implementation.

Rules:

- use the OS shell to open items;
- keep launch logic isolated;
- add logging/error handling only when a real failure mode is observed.

### tests/DesktopOrganizer.Core.Tests/WorkspaceTests.cs

This is the seed test file.

Add tests here when new domain behavior is introduced.

Do not test WPF rendering in this project.

## 5. Coding Conventions For Other Agents

All agents working on this repository should follow these rules:

1. Prefer the smallest change that satisfies the assigned module.
2. Do not move code across layers unless the current layer is clearly wrong.
3. Keep WPF concerns in `DesktopOrganizer.App`.
4. Keep persistence and shell integration in `DesktopOrganizer.Infrastructure`.
5. Keep domain models and interfaces in `DesktopOrganizer.Core`.
6. Add tests only for domain logic or deterministic infrastructure behavior.
7. Avoid introducing new packages unless a concrete module requires one.
8. Match the current naming and folder structure exactly.

## 6. Suggested Module Split For Parallel Work

These are safe parallel tracks for other agents:

- Agent A: group container UI in `App/Views` and `App/ViewModels`
- Agent B: drag/drop ingestion flow in `App` plus interfaces in `Core`
- Agent C: workspace persistence enhancements in `Infrastructure/Persistence`
- Agent D: shell launching, shortcut parsing, and folder portal support in `Infrastructure/Shell`
- Agent E: domain rules and unit tests in `Core` and `Core.Tests`

## 7. Next Milestone

The next implementation milestone should add:

1. create/remove/rename group commands;
2. drag local files and shortcuts into a group;
3. save workspace after layout changes;
4. open items from the UI.
