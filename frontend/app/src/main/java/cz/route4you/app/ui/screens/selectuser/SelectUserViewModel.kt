package cz.route4you.app.ui.screens.selectuser

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cz.route4you.app.data.UserRepository
import cz.route4you.app.data.local.UserPrefs
import cz.route4you.app.domain.User
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

data class SelectUserState(
    val isLoading: Boolean = false,
    val users: List<User> = emptyList(),
    val error: String? = null
)

class SelectUserViewModel(
    private val repo: UserRepository,
    private val prefs: UserPrefs
) : ViewModel() {

    private val _state = MutableStateFlow(SelectUserState())
    val state: StateFlow<SelectUserState> = _state

    fun load() {
        viewModelScope.launch {
            _state.value = SelectUserState(isLoading = true)
            try {
                val users = repo.getUsers()
                _state.value = SelectUserState(users = users)
            } catch (e: Exception) {
                _state.value = SelectUserState(error = e.message ?: "Network error")
            }
        }
    }

    fun selectUser(userId: String, onDone: () -> Unit) {
        viewModelScope.launch {
            prefs.setUserId(userId)
            onDone()
        }
    }
}