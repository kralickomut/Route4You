package cz.route4you.app.ui.screens.editarea

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cz.route4you.app.data.AreaRepository
import cz.route4you.app.data.remote.UpdateAreaDto
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

data class EditAreaState(
    val isLoading: Boolean = false,
    val isSaving: Boolean = false,
    val id: String = "",
    val name: String = "",
    val type: String = "",
    val error: String? = null
)

class EditAreaViewModel(private val repo: AreaRepository) : ViewModel() {
    private val _state = MutableStateFlow(EditAreaState())
    val state: StateFlow<EditAreaState> = _state

    fun load(areaId: String) {
        viewModelScope.launch {
            _state.value = EditAreaState(isLoading = true)
            try {
                val a = repo.getArea(areaId)
                _state.value = EditAreaState(
                    id = a.id,
                    name = a.name,
                    type = a.type
                )
            } catch (e: Exception) {
                _state.value = EditAreaState(error = e.message ?: "Load failed")
            }
        }
    }

    fun setName(v: String) {
        _state.value = _state.value.copy(name = v)
    }

    fun save(onSaved: () -> Unit) {
        val s = _state.value
        val newName = s.name.trim()
        if (newName.isBlank()) {
            _state.value = s.copy(error = "Name is required")
            return
        }

        viewModelScope.launch {
            _state.value = s.copy(isSaving = true, error = null)
            try {
                repo.update(
                    UpdateAreaDto(
                        id = s.id,
                        name = newName,
                        type = s.type,
                        latitude = null,
                        longitude = null
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