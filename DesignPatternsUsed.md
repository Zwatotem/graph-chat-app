# graph-chat-app
A simple client-server chat app, that remembers and displays which message replies to which.

## Design Patterns used

### MVVM

Whole [GUIChatClient](GUIChatClient/) is constructed with MVVM pattern. It has 3 layers, where model classes are situated in the [ChatModel](ChatModel/) project, and [View](GUIChatClient/View) and [Viewmodel](GUIChatClient/ViewModel) classes are in corresponding folders.

### Bridge

Project aims to be opened for future addition of chat messages that are not necesserly text-only. Therefore there is a generic [interface](ChatModel/Message/Content/IMessageContent.cs) for message content. To make content displaying and composing platform-agnostic, these functionalities are detached into [IContentViewModelProvider](ChatModel/Message/Content/IContentViewModelProvider.cs), which has implementation as [WPFContentViewModelProvider](GUIChatClient/ViewModel/WPFContentViewModelProvider.cs).

### [Singleton](GUIChatClient/App.xaml.cs)

App class is a singleton. It has only one private constructor and the only way to instantiate it is through static attribute Current. That way we're sure there is only ever one App class in the entire application. Other classes aren't encapsulated as singletons, instead they are attached to the App class, so that they can be used in multiplicity for testing, or if the case requires it. 

### Observer

A number of [ViewModel classes](GUIChatClient/ViewModel) use observer pattern in a form of C# built-in events for synchronization with the view.

### Strategy

Each operation type done at a server is handled as a [separate strategy](ChatServer/HandleStrategies). This allows for easy extensibility of this pool of tasks.

### Facade

All Net Socets IO operation are hidden behind a convinient [facade](ChatServer/ConcreteIOSocketFacade.cs)

### Adapter

Serialization and deserialization is handled with an [adapter classes](ChatModel/Util/ConcreteSerializer.cs), for built-in binary formatter.

### Composite

Viewmodel structure for displaying and sending messages is  built out of composites, where [MessageViewerViewModels](GUIChatClient/ViewModel/MessageViewerViewModel.cs) and [MessageCompositorViewModels](GUIChatClient/ViewModel/MessageCompositorViewModel.cs) are part of the same hierarchy, first being expandable and second being terminal elements. Their refresing is propagated upwards.