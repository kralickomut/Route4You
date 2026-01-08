package cz.route4you.app.ui.screens.areas

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cz.route4you.app.data.AreaRepository
import cz.route4you.app.domain.Area
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

data class BreadcrumbItem(
    val areaId: String?,   // null = root
    val title: String
)

data class AreasState(
    val isLoading: Boolean = false,
    val areas: List<Area> = emptyList(),
    val breadcrumbs: List<BreadcrumbItem> = emptyList(),
    val title: String = "Oblasti",
    val currentAreaId: String? = null,
    val currentAreaType: String? = null,
    val error: String? = null
)

class AreasViewModel(private val repo: AreaRepository) : ViewModel() {

    private val _state = MutableStateFlow(AreasState())
    val state: StateFlow<AreasState> = _state

    fun load(parentId: String?) {
        viewModelScope.launch {
            _state.value = _state.value.copy(isLoading = true, error = null)

            try {
                // 1) children
                val children = repo.getAreas(parentId)

                // 2) breadcrumbs + title
                if (parentId == null) {
                    _state.value = AreasState(
                        isLoading = false,
                        areas = children,
                        title = "Oblasti",
                        breadcrumbs = listOf(BreadcrumbItem(null, "Root"))
                    )
                } else {
                    val current = repo.getArea(parentId)

                    val crumbs = buildList {
                        add(BreadcrumbItem(null, "Root"))
                        for (i in current.pathIds.indices) {
                            add(BreadcrumbItem(current.pathIds[i], current.pathNames.getOrNull(i) ?: ""))
                        }
                        add(BreadcrumbItem(current.id, current.name))
                    }.filter { it.title.isNotBlank() }

                    _state.value = AreasState(
                        isLoading = false,
                        areas = children,
                        title = current.name,
                        breadcrumbs = crumbs,
                        currentAreaType = current.type
                    )
                }
            } catch (e: Exception) {
                _state.value = AreasState(
                    isLoading = false,
                    areas = emptyList(),
                    breadcrumbs = listOf(BreadcrumbItem(null, "Root")),
                    title = "Oblasti",
                    error = e.message ?: "Load failed"
                )
            }
        }
    }

    fun delete(areaId: String, parentId: String?) {
        viewModelScope.launch {
            try {
                repo.deleteArea(areaId)
                // po smazání jen reload aktuálního parent listu
                load(parentId)
            } catch (e: Exception) {

            }
        }
    }
}