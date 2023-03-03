// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.HtmlRendering;
using Microsoft.AspNetCore.Components.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Components.Web;

/// <summary>
/// Provides a mechanism for rendering components non-interactively as HTML markup.
/// </summary>
public sealed class HtmlRenderer : IDisposable, IAsyncDisposable
{
    private readonly IServiceProvider _services;
    private readonly HtmlRendererCore _passiveHtmlRenderer;

    /// <summary>
    /// Constructs an instance of <see cref="HtmlRenderer"/>.
    /// </summary>
    /// <param name="services">The services to use when rendering components.</param>
    /// <param name="loggerFactory">The logger factory to use.</param>
    public HtmlRenderer(IServiceProvider services, ILoggerFactory loggerFactory)
    {
        var componentActivator = services.GetService<IComponentActivator>() ?? DefaultComponentActivator.Instance;
        _services = services;
        _passiveHtmlRenderer = new HtmlRendererCore(services, loggerFactory, componentActivator);
    }

    /// <inheritdoc />
    public void Dispose()
        => _passiveHtmlRenderer.Dispose();

    /// <inheritdoc />
    public ValueTask DisposeAsync()
        => _passiveHtmlRenderer.DisposeAsync();

    /// <summary>
    /// Gets the <see cref="Components.Dispatcher" /> associated with this instance. Any calls to
    /// <see cref="RenderComponentAsync{TComponent}()"/> or <see cref="BeginRenderingComponent{TComponent}()"/>
    /// must be performed using this <see cref="Components.Dispatcher" />.
    /// </summary>
    public Dispatcher Dispatcher => _passiveHtmlRenderer.Dispatcher;

    /// <summary>
    /// Adds an instance of the specified component and instructs it to render. The resulting content represents the
    /// initial synchronous rendering output, which may later change. To wait for the component hierarchy to complete
    /// any asynchronous operations such as loading, use <see cref="HtmlComponent.WaitForQuiescenceAsync"/> before
    /// reading content from the <see cref="HtmlComponent"/>.
    /// </summary>
    /// <typeparam name="TComponent">The component type.</typeparam>
    /// <returns>An <see cref="HtmlComponent"/> instance representing the render output.</returns>
    public HtmlComponent BeginRenderingComponent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>() where TComponent : IComponent
        => _passiveHtmlRenderer.BeginRenderingComponentAsync(typeof(TComponent), ParameterView.Empty);

    /// <summary>
    /// Adds an instance of the specified component and instructs it to render. The resulting content represents the
    /// initial synchronous rendering output, which may later change. To wait for the component hierarchy to complete
    /// any asynchronous operations such as loading, use <see cref="HtmlComponent.WaitForQuiescenceAsync"/> before
    /// reading content from the <see cref="HtmlComponent"/>.
    /// </summary>
    /// <typeparam name="TComponent">The component type.</typeparam>
    /// <param name="parameters">Parameters for the component.</param>
    /// <returns>An <see cref="HtmlComponent"/> instance representing the render output.</returns>
    public HtmlComponent BeginRenderingComponent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(
        ParameterView parameters) where TComponent : IComponent
        => _passiveHtmlRenderer.BeginRenderingComponentAsync(typeof(TComponent), parameters);

    /// <summary>
    /// Adds an instance of the specified component and instructs it to render. The resulting content represents the
    /// initial synchronous rendering output, which may later change. To wait for the component hierarchy to complete
    /// any asynchronous operations such as loading, use <see cref="HtmlComponent.WaitForQuiescenceAsync"/> before
    /// reading content from the <see cref="HtmlComponent"/>.
    /// </summary>
    /// <param name="componentType">The component type. This must implement <see cref="IComponent"/>.</param>
    /// <returns>An <see cref="HtmlComponent"/> instance representing the render output.</returns>
    public HtmlComponent BeginRenderingComponent(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType)
        => _passiveHtmlRenderer.BeginRenderingComponentAsync(componentType, ParameterView.Empty);

    /// <summary>
    /// Adds an instance of the specified component and instructs it to render. The resulting content represents the
    /// initial synchronous rendering output, which may later change. To wait for the component hierarchy to complete
    /// any asynchronous operations such as loading, use <see cref="HtmlComponent.WaitForQuiescenceAsync"/> before
    /// reading content from the <see cref="HtmlComponent"/>.
    /// </summary>
    /// <param name="componentType">The component type. This must implement <see cref="IComponent"/>.</param>
    /// <param name="parameters">Parameters for the component.</param>
    /// <returns>An <see cref="HtmlComponent"/> instance representing the render output.</returns>
    public HtmlComponent BeginRenderingComponent(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType,
        ParameterView parameters)
        => _passiveHtmlRenderer.BeginRenderingComponentAsync(componentType, parameters);

    /// <summary>
    /// Adds an instance of the specified component and instructs it to render, waiting
    /// for the component hierarchy to complete asynchronous tasks such as loading.
    /// </summary>
    /// <typeparam name="TComponent">The component type.</typeparam>
    /// <returns>A task that completes with <see cref="HtmlComponent"/> once the component hierarchy has completed any asynchronous tasks such as loading.</returns>
    public Task<HtmlComponent> RenderComponentAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>() where TComponent : IComponent
        => RenderComponentAsync<TComponent>(ParameterView.Empty);

    /// <summary>
    /// Adds an instance of the specified component and instructs it to render, waiting
    /// for the component hierarchy to complete asynchronous tasks such as loading.
    /// </summary>
    /// <param name="componentType">The component type. This must implement <see cref="IComponent"/>.</param>
    /// <returns>A task that completes with <see cref="HtmlComponent"/> once the component hierarchy has completed any asynchronous tasks such as loading.</returns>
    public Task<HtmlComponent> RenderComponentAsync(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType)
        => RenderComponentAsync(componentType, ParameterView.Empty);

    /// <summary>
    /// Adds an instance of the specified component and instructs it to render, waiting
    /// for the component hierarchy to complete asynchronous tasks such as loading.
    /// </summary>
    /// <typeparam name="TComponent">The component type.</typeparam>
    /// <param name="parameters">Parameters for the component.</param>
    /// <returns>A task that completes with <see cref="HtmlComponent"/> once the component hierarchy has completed any asynchronous tasks such as loading.</returns>
    public Task<HtmlComponent> RenderComponentAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(
        ParameterView parameters) where TComponent : IComponent
        => RenderComponentAsync(typeof (TComponent), parameters);

    /// <summary>
    /// Adds an instance of the specified component and instructs it to render, waiting
    /// for the component hierarchy to complete asynchronous tasks such as loading.
    /// </summary>
    /// <param name="componentType">The component type. This must implement <see cref="IComponent"/>.</param>
    /// <param name="parameters">Parameters for the component.</param>
    /// <returns>A task that completes with <see cref="HtmlComponent"/> once the component hierarchy has completed any asynchronous tasks such as loading.</returns>
    public async Task<HtmlComponent> RenderComponentAsync(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType,
        ParameterView parameters)
    {
        var content = BeginRenderingComponent(componentType, parameters);
        await content.WaitForQuiescenceAsync();
        return content;
    }

    /// <summary>
    /// Persists the component application state into the given <see cref="IPersistentComponentStateStore"/>.
    /// </summary>
    /// <param name="store">The <see cref="IPersistentComponentStateStore"/> to persist the state into.</param>
    /// <returns>A <see cref="Task"/> that will complete when the state has been persisted.</returns>
    public Task PersistComponentStateAsync(IPersistentComponentStateStore store)
    {
        // TODO: I don't think this is a good thing because, in class library scenarios, people aren't going to have
        // ComponentStatePersistenceManager in their DI container, and we don't even want them to add it manually
        // as it's in .Infrastructure. We'd probably have to start telling everyone to call some new IServiceCollection
        // extension method to register "required services" which is a bit lame because today we don't require any
        // manadatory services besides logging.
        var manager = _services.GetRequiredService<ComponentStatePersistenceManager>();
        return manager.PersistStateAsync(store, _passiveHtmlRenderer);
    }
}
