package cz.route4you.app.ui.nav

object NavRoutes {
    const val SelectUser = "selectUser"

    // Areas: root + child
    const val AreasRoot = "areas"
    const val Areas = "areas/{parentId}"

    fun areasRoute(parentId: String?) =
        parentId?.let { "areas/$it" } ?: AreasRoot

    // Routes
    const val Routes = "routes/{areaId}"
    fun routesRoute(areaId: String) = "routes/$areaId"

    const val RouteDetail = "route/{routeId}"
    fun routeDetailRoute(routeId: String) = "route/$routeId"

    const val RecordAscent = "ascent/record/{routeId}"
    fun recordAscentRoute(routeId: String) = "ascent/record/$routeId"

    // Create area (ponecháme jak máš)
    const val CreateArea = "areas/create/{parentId}"
    fun createAreaRoute(parentId: String?) = "areas/create/${parentId ?: "null"}"

    const val CreateRoute = "routes/create/{areaId}"
    fun createRouteRoute(areaId: String) = "routes/create/$areaId"

    const val EditArea = "areas/edit/{areaId}"
    fun editAreaRoute(areaId: String) = "areas/edit/$areaId"

    const val EditRoute = "routes/edit/{routeId}"
    fun editRouteRoute(routeId: String) = "routes/edit/$routeId"
}