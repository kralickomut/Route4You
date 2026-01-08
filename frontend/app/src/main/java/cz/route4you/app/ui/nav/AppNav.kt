package cz.route4you.app.ui.nav

import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import cz.route4you.app.data.*
import cz.route4you.app.data.local.UserPrefs
import cz.route4you.app.data.remote.ApiClient
import cz.route4you.app.ui.screens.areas.AreasScreen
import cz.route4you.app.ui.screens.areas.AreasViewModel
import cz.route4you.app.ui.screens.createarea.CreateAreaScreen
import cz.route4you.app.ui.screens.createarea.CreateAreaViewModel
import cz.route4you.app.ui.screens.createroute.CreateRouteScreen
import cz.route4you.app.ui.screens.createroute.CreateRouteViewModel
import cz.route4you.app.ui.screens.editarea.EditAreaScreen
import cz.route4you.app.ui.screens.editarea.EditAreaViewModel
import cz.route4you.app.ui.screens.editroute.EditRouteScreen
import cz.route4you.app.ui.screens.editroute.EditRouteViewModel
import cz.route4you.app.ui.screens.recordascent.RecordAscentScreen
import cz.route4you.app.ui.screens.recordascent.RecordAscentViewModel
import cz.route4you.app.ui.screens.routes.RoutesScreen
import cz.route4you.app.ui.screens.routes.RoutesViewModel
import cz.route4you.app.ui.screens.routedetail.RouteDetailScreen
import cz.route4you.app.ui.screens.routedetail.RouteDetailViewModel
import cz.route4you.app.ui.screens.selectuser.SelectUserScreen
import cz.route4you.app.ui.screens.selectuser.SelectUserViewModel

@Composable
fun AppNav(modifier: Modifier = Modifier) {
    val nav = rememberNavController()
    val ctx = LocalContext.current

    val prefs = remember { UserPrefs(ctx) }

    val userRepo = remember { UserRepository(ApiClient.api) }
    val selectUserVm = remember { SelectUserViewModel(userRepo, prefs) }

    val areaRepo = remember { AreaRepository(ApiClient.api) }
    val areasVm = remember { AreasViewModel(areaRepo) }

    val routeRepo = remember { RouteRepository(ApiClient.api) }
    val routesVm = remember { RoutesViewModel(routeRepo) }

    val createAreaVm = remember { CreateAreaViewModel(areaRepo) }
    val createRouteVm = remember { CreateRouteViewModel(routeRepo, prefs) }

    val editAreaVm = remember { EditAreaViewModel(areaRepo) }
    val editRouteVm = remember { EditRouteViewModel(routeRepo) }

    val ascentRepo = remember { AscentRepository(ApiClient.api) }
    val recordVm = remember { RecordAscentViewModel(ascentRepo, prefs) }
    val routeDetailVm = remember { RouteDetailViewModel(routeRepo, areaRepo) }

    NavHost(
        navController = nav,
        startDestination = NavRoutes.SelectUser,
        modifier = modifier
    ) {
        composable(NavRoutes.SelectUser) {
            SelectUserScreen(vm = selectUserVm) {
                nav.navigate(NavRoutes.AreasRoot)
            }
        }

        composable(NavRoutes.RecordAscent) { backStackEntry ->
            val routeId = backStackEntry.arguments?.getString("routeId")!!

            RecordAscentScreen(
                vm = recordVm,
                routeId = routeId,
                onBack = { nav.navigateUp() },
                onSaved = {
                    nav.navigateUp() // zpět na detail
                }
            )
        }

        composable(NavRoutes.CreateArea) { backStackEntry ->
            val pidRaw = backStackEntry.arguments?.getString("parentId")
            val parentId = pidRaw?.takeUnless { it == "null" }

            CreateAreaScreen(
                vm = createAreaVm,
                parentId = parentId,
                onBack = { nav.navigateUp() },
                onSaved = {
                    // refresh Areas list
                    nav.previousBackStackEntry?.savedStateHandle?.set("areasRefresh", (System.currentTimeMillis().toInt()))
                    nav.navigateUp()
                }
            )
        }

        composable(NavRoutes.EditArea) { backStackEntry ->
            val areaId = backStackEntry.arguments?.getString("areaId")!!
            EditAreaScreen(
                vm = editAreaVm,
                areaId = areaId,
                onBack = { nav.navigateUp() },
                onSaved = { nav.navigateUp() }
            )
        }

        composable(NavRoutes.EditRoute) { backStackEntry ->
            val routeId = backStackEntry.arguments?.getString("routeId")!!
            EditRouteScreen(
                vm = editRouteVm,
                routeId = routeId,
                onBack = { nav.navigateUp() },
                onSaved = { nav.navigateUp() }
            )
        }

        composable(NavRoutes.AreasRoot) {
            AreasScreen(
                vm = areasVm,
                parentId = null,
                onBack = { nav.navigateUp() },
                onBreadcrumbClick = { id -> nav.navigate(NavRoutes.areasRoute(id)) },
                onAreaClick = { area ->
                    val t = area.type.lowercase()
                    val isLeaf = t != "country" && t != "region"
                    if (isLeaf) nav.navigate(NavRoutes.routesRoute(area.id))
                    else nav.navigate(NavRoutes.areasRoute(area.id))
                },
                onLeafArea = { leafId -> nav.navigate(NavRoutes.routesRoute(leafId)) },
                onCreateClick = { pid -> nav.navigate(NavRoutes.createAreaRoute(pid)) },
                onEditAreaClick = { areaId -> nav.navigate(NavRoutes.editAreaRoute(areaId)) },
                onDeleteAreaClick = { areaId -> areasVm.delete(areaId, null) }
            )
        }

        composable(NavRoutes.Areas) { backStackEntry ->
            val parentId = backStackEntry.arguments?.getString("parentId")!!

            AreasScreen(
                vm = areasVm,
                parentId = parentId,
                onBack = { nav.navigateUp() },
                onBreadcrumbClick = { id -> nav.navigate(NavRoutes.areasRoute(id)) },
                onAreaClick = { area ->
                    val t = area.type.lowercase()
                    val isLeaf = t != "country" && t != "region"
                    if (isLeaf) nav.navigate(NavRoutes.routesRoute(area.id))
                    else nav.navigate(NavRoutes.areasRoute(area.id))
                },
                onLeafArea = { leafId -> nav.navigate(NavRoutes.routesRoute(leafId)) },
                onCreateClick = { pid -> nav.navigate(NavRoutes.createAreaRoute(pid)) },
                onEditAreaClick = { areaId -> nav.navigate(NavRoutes.editAreaRoute(areaId)) },
                onDeleteAreaClick = { areaId -> areasVm.delete(areaId, parentId) }
            )
        }

        composable(NavRoutes.Routes) { backStackEntry ->
            val areaId = backStackEntry.arguments?.getString("areaId")!!

            RoutesScreen(
                vm = routesVm,
                areaId = areaId,
                onBack = { nav.navigateUp() },
                onRouteClick = { routeId -> nav.navigate(NavRoutes.routeDetailRoute(routeId)) },
                onCreateRouteClick = { aId -> nav.navigate(NavRoutes.createRouteRoute(aId)) },

                // 3 dots
                onEditRouteClick = { routeId -> nav.navigate(NavRoutes.editRouteRoute(routeId)) },
                onDeleteRouteClick = { routeId ->
                    routesVm.delete(routeId, areaId)
                }
            )
        }

        composable(NavRoutes.CreateRoute) { backStackEntry ->
            val areaId = backStackEntry.arguments?.getString("areaId")!!

            CreateRouteScreen(
                vm = createRouteVm,
                areaId = areaId,
                onBack = { nav.navigateUp() },
                onSaved = { _ ->
                    nav.previousBackStackEntry?.savedStateHandle?.set("routesRefresh", (System.currentTimeMillis().toInt()))
                    nav.navigateUp()
                }
            )
        }

        composable(NavRoutes.RouteDetail) { backStackEntry ->
            val routeId = backStackEntry.arguments?.getString("routeId")!!

            RouteDetailScreen(
                vm = routeDetailVm,
                routeId = routeId,
                onBack = { nav.navigateUp() },
                onBreadcrumbClick = { areaId -> nav.navigate(NavRoutes.areasRoute(areaId)) },
                onRecordAscentClick = { rid ->
                    nav.navigate(NavRoutes.recordAscentRoute(rid))
                }
            )
        }
    }
}