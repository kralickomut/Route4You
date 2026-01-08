package cz.route4you.app.ui.screens.routes

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cz.route4you.app.data.RouteRepository
import cz.route4you.app.domain.ClimbRoute
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

data class RoutesState(
    val isLoading: Boolean = false,
    val routes: List<ClimbRoute> = emptyList(),
    val error: String? = null
)

class RoutesViewModel(private val repo: RouteRepository) : ViewModel() {

    private val _state = MutableStateFlow(RoutesState())
    val state: StateFlow<RoutesState> = _state

    fun load(areaId: String) {
        viewModelScope.launch {
            _state.value = RoutesState(isLoading = true)
            try {
                val routes = repo.getRoutesByArea(areaId)
                _state.value = RoutesState(routes = routes)
            } catch (e: Exception) {
                _state.value = RoutesState(error = e.message ?: "Network error")
            }
        }
    }

    fun delete(routeId: String, areaId: String) {
        viewModelScope.launch {
            try {
                repo.deleteRoute(routeId)
                load(areaId)                // refresh list
            } catch (e: Exception) {
                _state.value = _state.value.copy(error = e.message ?: "Delete failed")
            }
        }
    }
}