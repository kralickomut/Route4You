package cz.route4you.app.ui.screens.editroute

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cz.route4you.app.data.RouteRepository
import cz.route4you.app.data.remote.UpdateRouteDto
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

data class EditRouteState(
    val isLoading: Boolean = false,
    val isSaving: Boolean = false,
    val id: String = "",
    val name: String = "",
    val grade: String = "",
    val pitchesText: String = "1",
    val lengthText: String = "",
    val style: String = "Sport",
    val tagsText: String = "",
    val error: String? = null
)

class EditRouteViewModel(private val repo: RouteRepository) : ViewModel() {
    private val _state = MutableStateFlow(EditRouteState())
    val state: StateFlow<EditRouteState> = _state

    fun load(routeId: String) {
        viewModelScope.launch {
            _state.value = EditRouteState(isLoading = true)
            try {
                val r = repo.getRoute(routeId) // detail
                _state.value = EditRouteState(
                    id = r.id,
                    name = r.name,
                    grade = r.grade,
                    pitchesText = r.pitches.toString(),
                    lengthText = r.lengthMeters?.toString() ?: "",
                    style = r.style,
                    tagsText = r.tags.joinToString(", ")
                )
            } catch (e: Exception) {
                _state.value = EditRouteState(error = e.message ?: "Load failed")
            }
        }
    }

    fun setName(v: String) { _state.value = _state.value.copy(name = v) }
    fun setGrade(v: String) { _state.value = _state.value.copy(grade = v) }
    fun setPitches(v: String) { _state.value = _state.value.copy(pitchesText = v) }
    fun setLength(v: String) { _state.value = _state.value.copy(lengthText = v) }
    fun setStyle(v: String) { _state.value = _state.value.copy(style = v) }
    fun setTags(v: String) { _state.value = _state.value.copy(tagsText = v) }

    fun save(onSaved: () -> Unit) {
        val s = _state.value
        viewModelScope.launch {
            _state.value = s.copy(isSaving = true, error = null)
            try {
                val pitches = s.pitchesText.toIntOrNull()?.coerceAtLeast(1) ?: 1
                val length = s.lengthText.trim().takeIf { it.isNotBlank() }?.toIntOrNull()

                val tags = s.tagsText
                    .split(",")
                    .map { it.trim() }
                    .filter { it.isNotBlank() }

                repo.update(
                    UpdateRouteDto(
                        id = s.id,
                        name = s.name.trim(),
                        grade = s.grade.trim(),
                        pitches = pitches,
                        lengthMeters = length,
                        style = s.style.trim(),
                        tags = tags
                    )
                )

                _state.value = _state.value.copy(isSaving = false)
                onSaved()
            } catch (e: Exception) {
                _state.value = _state.value.copy(isSaving = false, error = e.message ?: "Save failed")
            }
        }
    }
}