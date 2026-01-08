package cz.route4you.app.domain

data class Area(
    val id: String,
    val name: String,
    val type: String,
    val parentId: String?,
    val childrenCount: Int,
    val routesCount: Int,
    val pathIds: List<String> = emptyList(),
    val pathNames: List<String> = emptyList()
)