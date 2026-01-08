package cz.route4you.app.ui.screens.routedetail

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cz.route4you.app.data.AreaRepository
import cz.route4you.app.data.RouteRepository
import cz.route4you.app.domain.RouteDetail
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

data class BreadcrumbItem(
    val areaId: String?,   // null = root
    val title: String
)

data class RouteDetailState(
    val isLoading: Boolean = false,
    val route: RouteDetail? = null,
    val breadcrumbs: List<BreadcrumbItem> = emptyList(),
    val error: String? = null
)

class RouteDetailViewModel(
    private val routes: RouteRepository,
    private val areas: AreaRepository
) : ViewModel() {

    private val _state = MutableStateFlow(RouteDetailState())
    val state: StateFlow<RouteDetailState> = _state

    fun load(routeId: String) {
        viewModelScope.launch {
            _state.value = RouteDetailState(isLoading = true)
            try {
                val route = routes.getRoute(routeId)

                // breadcrumb z area.pathIds + pathNames
                val area = areas.getArea(route.areaId)

                val crumbs = buildList {
                    add(BreadcrumbItem(null, "Root"))
                    for (i in area.pathIds.indices) {
                        add(BreadcrumbItem(area.pathIds[i], area.pathNames.getOrNull(i) ?: ""))
                    }
                    add(BreadcrumbItem(area.id, area.name))
                }.filter { it.title.isNotBlank() }

                _state.value = RouteDetailState(route = route, breadcrumbs = crumbs)
            } catch (e: Exception) {
                _state.value = RouteDetailState(error = e.message ?: "Load failed")
            }
        }
    }
}