import {type RouteConfig, index, route} from "@react-router/dev/routes";

export default [
    index("routes/home.tsx"),
    route("calendar", "routes/calendar.tsx"),
    route("join/:groupId", "routes/join.tsx"),    
    route("joinGroup", "routes/joinGroupPage.tsx")
] satisfies RouteConfig;
